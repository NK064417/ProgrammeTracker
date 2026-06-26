using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using ProgrammeTracker.Data;
using ProgrammeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ProgrammeTracker.Services
{
    public interface IEmailService
    {
        Task<(bool success, string message)> SendAsync(string toEmail, string toName, string subject, string body);
        Task<(bool success, string message)> SendTestAsync(string toEmail);
    }

    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ApplicationDbContext db, ILogger<EmailService> logger)
        {
            _db = db;
            _logger = logger;
        }

        private async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            var settings = await _db.AppSettings.ToListAsync();
            return settings.ToDictionary(s => s.Key, s => s.Value ?? "");
        }

        public async Task<(bool success, string message)> SendAsync(string toEmail, string toName, string subject, string body)
        {
            try
            {
                var cfg = await GetSettingsAsync();

                if (!cfg.TryGetValue(AppSettingKeys.SmtpHost, out var host) || string.IsNullOrWhiteSpace(host))
                    return (false, "SMTP not configured");

                _ = int.TryParse(cfg.GetValueOrDefault(AppSettingKeys.SmtpPort, "587"), out int port);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Programme Tracker", cfg.GetValueOrDefault(AppSettingKeys.SmtpFromAddress)));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable);

                var user = cfg.GetValueOrDefault(AppSettingKeys.SmtpUsername, "");
                var pass = cfg.GetValueOrDefault(AppSettingKeys.SmtpPassword, "");
                if (!string.IsNullOrWhiteSpace(user))
                    await client.AuthenticateAsync(user, pass);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {Email}: {Subject}", toEmail, subject);
                return (true, "Sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return (false, ex.Message);
            }
        }

        public async Task<(bool success, string message)> SendTestAsync(string toEmail)
        {
            return await SendAsync(toEmail, toEmail, "Programme Tracker — Test Email",
                "<p>Your SMTP configuration is working correctly.</p>");
        }
    }

    // ── Notification Background Service ──
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<NotificationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessNotificationsAsync();

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var intervalSetting = await db.AppSettings.FindAsync(AppSettingKeys.CheckIntervalSeconds);
                int intervalSeconds = int.TryParse(intervalSetting?.Value, out int v) ? v : 3600;

                await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
            }
        }

        public async Task ProcessNotificationsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var enabledSetting = await db.AppSettings.FindAsync(AppSettingKeys.NotifyEnabled);
            if (enabledSetting?.Value?.ToLower() != "true") return;

            var today = DateTime.UtcNow.Date;

            var deliverables = await db.Deliverables
                .Include(d => d.Programme)
                .Include(d => d.AssignedTo)
                .Where(d => d.Status != "Complete" && d.AssignedToId != null)
                .ToListAsync();

            foreach (var d in deliverables)
            {
                // Auto-mark overdue
                if (d.DaysRemaining < 0 && d.Status != "Overdue")
                {
                    d.Status = "Overdue";
                    d.UpdatedAt = DateTime.UtcNow;
                }

                // Check if notification needed
                if (d.DaysRemaining > d.NotifyDaysBefore) continue;

                // Check if already notified today
                bool alreadySent = await db.NotificationLogs.AnyAsync(n =>
                    n.DeliverableId == d.Id &&
                    n.SentToUserId == d.AssignedToId &&
                    n.SentAt.Date == today);

                if (alreadySent) continue;

                var user = d.AssignedTo!;
                string statusTag = d.IsOverdue ? "⚠️ OVERDUE" : $"Due in {d.DaysRemaining} day(s)";
                string subject = d.IsOverdue
                    ? $"[OVERDUE] {d.Title} — {d.Programme?.ClientName}"
                    : $"[Reminder] {d.Title} due in {d.DaysRemaining} day(s)";

                string body = $"""
                    <h2 style="color:#2c3e50">Programme Tracker Notification</h2>
                    <p><strong>Deliverable:</strong> {d.Title}</p>
                    <p><strong>Programme:</strong> {d.Programme?.ProgrammeName}</p>
                    <p><strong>Client:</strong> {d.Programme?.ClientName}</p>
                    <p><strong>Due Date:</strong> {d.DueDate:dd MMM yyyy}</p>
                    <p><strong>Status:</strong> <span style="color:{(d.IsOverdue ? "red" : "orange")}">{statusTag}</span></p>
                    <hr/>
                    <p style="color:#888;font-size:12px">Programme Tracker System — automated notification</p>
                    """;

                var (success, message) = await emailService.SendAsync(user.Email!, user.FullName, subject, body);

                db.NotificationLogs.Add(new NotificationLog
                {
                    DeliverableId = d.Id,
                    SentToUserId = d.AssignedToId,
                    SentAt = DateTime.UtcNow,
                    Status = success ? "Sent" : "Failed",
                    Message = message
                });
            }

            await db.SaveChangesAsync();
            _logger.LogInformation("Notification check completed at {Time}", DateTime.UtcNow);
        }
    }
}

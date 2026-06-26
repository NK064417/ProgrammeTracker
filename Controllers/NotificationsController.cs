using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammeTracker.Data;
using ProgrammeTracker.Models;
using ProgrammeTracker.Services;
using ProgrammeTracker.ViewModels;

namespace ProgrammeTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly NotificationBackgroundService _notificationService;

        public NotificationsController(ApplicationDbContext db, NotificationBackgroundService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _db.NotificationLogs
                .Include(n => n.Deliverable).ThenInclude(d => d!.Programme)
                .Include(n => n.SentToUser)
                .OrderByDescending(n => n.SentAt)
                .Take(100)
                .ToListAsync();
            return View(logs);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TriggerNow()
        {
            await _notificationService.ProcessNotificationsAsync();
            TempData["Success"] = "Notification check triggered successfully.";
            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;

        public SettingsController(ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await LoadSettingsAsync();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SmtpSettingsViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await SaveSettingAsync(AppSettingKeys.SmtpHost, vm.SmtpHost);
            await SaveSettingAsync(AppSettingKeys.SmtpPort, vm.SmtpPort.ToString());
            await SaveSettingAsync(AppSettingKeys.SmtpUsername, vm.SmtpUsername ?? "");
            if (!string.IsNullOrWhiteSpace(vm.SmtpPassword))
                await SaveSettingAsync(AppSettingKeys.SmtpPassword, vm.SmtpPassword);
            await SaveSettingAsync(AppSettingKeys.SmtpFromAddress, vm.SmtpFromAddress);
            await SaveSettingAsync(AppSettingKeys.NotifyEnabled, vm.NotifyEnabled.ToString().ToLower());
            await SaveSettingAsync(AppSettingKeys.CheckIntervalSeconds, vm.CheckIntervalSeconds.ToString());

            TempData["Success"] = "Settings saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTest(string testEmail)
        {
            var (success, message) = await _emailService.SendTestAsync(testEmail);
            TempData[success ? "Success" : "Error"] = success
                ? $"Test email sent to {testEmail}."
                : $"Test failed: {message}";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SmtpSettingsViewModel> LoadSettingsAsync()
        {
            var settings = (await _db.AppSettings.ToListAsync())
                .ToDictionary(s => s.Key, s => s.Value ?? "");

            return new SmtpSettingsViewModel
            {
                SmtpHost = settings.GetValueOrDefault(AppSettingKeys.SmtpHost, ""),
                SmtpPort = int.TryParse(settings.GetValueOrDefault(AppSettingKeys.SmtpPort), out int p) ? p : 587,
                SmtpUsername = settings.GetValueOrDefault(AppSettingKeys.SmtpUsername, ""),
                SmtpFromAddress = settings.GetValueOrDefault(AppSettingKeys.SmtpFromAddress, ""),
                NotifyEnabled = settings.GetValueOrDefault(AppSettingKeys.NotifyEnabled, "true") == "true",
                CheckIntervalSeconds = int.TryParse(settings.GetValueOrDefault(AppSettingKeys.CheckIntervalSeconds), out int i) ? i : 3600
            };
        }

        private async Task SaveSettingAsync(string key, string value)
        {
            var setting = await _db.AppSettings.FindAsync(key);
            if (setting == null)
                _db.AppSettings.Add(new AppSettings { Key = key, Value = value });
            else
                setting.Value = value;
            await _db.SaveChangesAsync();
        }
    }
}

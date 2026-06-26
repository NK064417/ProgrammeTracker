using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgrammeTracker.Models;

namespace ProgrammeTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ProgrammeType> ProgrammeTypes { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Deliverable> Deliverables { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── Users ──
            builder.Entity<ApplicationUser>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
            });

            // ── Programmes ──
            // SQL Server does not allow multiple SET NULL cascade paths to the same table.
            // All User FKs on Programmes use ClientNoAction — nulling is handled in code (UsersController).
            builder.Entity<Programme>(e =>
            {
                e.HasOne(p => p.ProgrammeType)
                    .WithMany(t => t.Programmes)
                    .HasForeignKey(p => p.ProgrammeTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.AccountManager)
                    .WithMany(u => u.ManagedProgrammes)
                    .HasForeignKey(p => p.AccountManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.HasOne(p => p.Coordinator)
                    .WithMany(u => u.CoordinatedProgrammes)
                    .HasForeignKey(p => p.CoordinatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.HasOne(p => p.Facilitator)
                    .WithMany(u => u.FacilitatedProgrammes)
                    .HasForeignKey(p => p.FacilitatorId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.HasOne(p => p.TalentManager)
                    .WithMany(u => u.TalentManagedProgrammes)
                    .HasForeignKey(p => p.TalentManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.HasOne(p => p.Payroll)
                    .WithMany()
                    .HasForeignKey(p => p.PayrollId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                e.HasOne(p => p.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Deliverables ──
            builder.Entity<Deliverable>(e =>
            {
                e.HasOne(d => d.Programme)
                    .WithMany(p => p.Deliverables)
                    .HasForeignKey(d => d.ProgrammeId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(d => d.AssignedTo)
                    .WithMany(u => u.AssignedDeliverables)
                    .HasForeignKey(d => d.AssignedToId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ── NotificationLogs ──
            builder.Entity<NotificationLog>(e =>
            {
                e.HasOne(n => n.Deliverable)
                    .WithMany(d => d.NotificationLogs)
                    .HasForeignKey(n => n.DeliverableId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(n => n.SentToUser)
                    .WithMany(u => u.NotificationLogs)
                    .HasForeignKey(n => n.SentToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ── Seed ProgrammeTypes ──
            builder.Entity<ProgrammeType>().HasData(
                new ProgrammeType { Id = 1, Name = "Learnership", DefaultPeriodMonths = 12, IsActive = true },
                new ProgrammeType { Id = 2, Name = "QTCO", DefaultPeriodMonths = 18, IsActive = true },
                new ProgrammeType { Id = 3, Name = "Skills Programme", DefaultPeriodMonths = 3, IsActive = true },
                new ProgrammeType { Id = 4, Name = "Internship", DefaultPeriodMonths = 12, IsActive = true },
                new ProgrammeType { Id = 5, Name = "Customised", DefaultPeriodMonths = null, IsActive = true }
            );

            // ── Seed default AppSettings ──
            builder.Entity<AppSettings>().HasData(
                new AppSettings { Key = AppSettingKeys.SmtpHost, Value = "" },
                new AppSettings { Key = AppSettingKeys.SmtpPort, Value = "587" },
                new AppSettings { Key = AppSettingKeys.SmtpUsername, Value = "" },
                new AppSettings { Key = AppSettingKeys.SmtpPassword, Value = "" },
                new AppSettings { Key = AppSettingKeys.SmtpFromAddress, Value = "" },
                new AppSettings { Key = AppSettingKeys.NotifyEnabled, Value = "true" },
                new AppSettings { Key = AppSettingKeys.CheckIntervalSeconds, Value = "3600" }
            );
        }
    }
}
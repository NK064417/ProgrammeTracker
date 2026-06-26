using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgrammeTracker.Models;

namespace ProgrammeTracker.ViewModels
{
    // ── Dashboard ──
    public class DashboardViewModel
    {
        public int TotalProgrammes { get; set; }
        public int ActiveProgrammes { get; set; }
        public int OverdueDeliverables { get; set; }
        public int DueSoonDeliverables { get; set; }
        public List<Programme> Programmes { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? FilterStatus { get; set; }
        public string? FilterType { get; set; }
    }

    // ── Programme Create/Edit (Phase 1) ──
    public class ProgrammePhase1ViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Delivery type is required")]
        [Display(Name = "Delivery Type")]
        public string DeliveryType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client name is required")]
        [MaxLength(150), Display(Name = "Client Name")]
        public string ClientName { get; set; } = string.Empty;

        [MaxLength(100), Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [MaxLength(100), Display(Name = "Contact Details")]
        public string? ContactDetails { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Stipend (R)")]
        public decimal? Stipend { get; set; }

        [MaxLength(50)]
        public string? Bursary { get; set; }

        [Required, Display(Name = "SETA Funded")]
        public string Funding { get; set; } = "No";

        [MaxLength(50), Display(Name = "EE Services")]
        public string? EEServices { get; set; }

        [MaxLength(50), Display(Name = "WSP Services")]
        public string? WSPServices { get; set; }

        // Learner Details
        [Display(Name = "Number of Learners")]
        public int? NumberOfLearners { get; set; }

        [Display(Name = "Absorption Assistance")]
        public string? AbsorptionAssistance { get; set; }

        [MaxLength(20), Display(Name = "Disability Ratio")]
        public string? DisabilityRatio { get; set; }

        [MaxLength(100), Display(Name = "Type of Disability")]
        public string? TypeOfDisability { get; set; }

        [Display(Name = "Male Capacity")]
        public int? MaleCapacity { get; set; }

        [Display(Name = "Female Capacity")]
        public int? FemaleCapacity { get; set; }

        [Display(Name = "Other Gender Capacity")]
        public int? OtherGenderCapacity { get; set; }

        [MaxLength(200), Display(Name = "EE Composition")]
        public string? EEComposition { get; set; }

        [Display(Name = "Preferred Province")]
        public string? PreferredProvince { get; set; }

        [Display(Name = "Account Manager")]
        public int? AccountManagerId { get; set; }

        // Skills Development
        [Required(ErrorMessage = "Programme type is required")]
        [Display(Name = "Programme Type")]
        public int ProgrammeTypeId { get; set; }

        [Required(ErrorMessage = "Programme name is required")]
        [MaxLength(200), Display(Name = "Programme Name")]
        public string ProgrammeName { get; set; } = string.Empty;

        [MaxLength(200), Display(Name = "Top-Up Skills Training")]
        public string? TopUpSkillsTraining { get; set; }

        [Display(Name = "Soft Skills Training")]
        public string? SoftSkillsTraining { get; set; }

        [Display(Name = "Reporting Frequency")]
        public string? ReportingFrequency { get; set; }

        [MaxLength(500), Display(Name = "Learner Required Attributes")]
        public string? LearnerRequiredAttributes { get; set; }

        // Periods
        [Display(Name = "Training Period (months)")]
        public int? TrainingPeriodMonths { get; set; }

        [Display(Name = "Hosting Period (months)")]
        public int? HostingPeriodMonths { get; set; }

        [Display(Name = "Workplace Period (months)")]
        public int? WorkplacePeriodMonths { get; set; }

        [Display(Name = "Total Period (months)")]
        public int? TotalPeriodMonths { get; set; }

        // Absenteeism
        [Display(Name = "Absenteeism Managed By")]
        public string? AbsenteeismManagedBy { get; set; }

        [Display(Name = "Replacement")]
        public string? ReplacementPolicy { get; set; }

        [MaxLength(50), Display(Name = "Replacement Timeframe")]
        public string? ReplacementTimeframe { get; set; }

        // Dropdowns
        public SelectList? DeliveryTypes { get; set; }
        public SelectList? ProgrammeTypes { get; set; }
        public SelectList? AccountManagers { get; set; }
        public SelectList? Provinces { get; set; }
        public SelectList? ReportingFrequencies { get; set; }
    }

    // ── Programme Phase 2 ──
    public class ProgrammePhase2ViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ProgrammeName { get; set; } = string.Empty;

        [Display(Name = "Project Coordinator")]
        public int? CoordinatorId { get; set; }

        [Display(Name = "Payroll")]
        public int? PayrollId { get; set; }

        [Display(Name = "Facilitator")]
        public int? FacilitatorId { get; set; }

        [Display(Name = "Talent Manager")]
        public int? TalentManagerId { get; set; }

        [MaxLength(100), Display(Name = "SETA Advisor Name")]
        public string? SETAAdvisorName { get; set; }

        [MaxLength(100), Display(Name = "SETA Advisor Contact")]
        public string? SETAAdvisorContact { get; set; }

        [Display(Name = "Learner Presentation 1")]
        public DateTime? LearnerPresentation1Date { get; set; }

        [Display(Name = "Learner Presentation 2")]
        public DateTime? LearnerPresentation2Date { get; set; }

        [Display(Name = "Learner Registration Deadline")]
        public DateTime? LearnerRegistrationDeadline { get; set; }

        [Display(Name = "Placement Date")]
        public DateTime? PlacementDate { get; set; }

        [Display(Name = "Soft Skills Start Date")]
        public DateTime? SoftSkillsStartDate { get; set; }

        [Display(Name = "Moderation Report Date")]
        public DateTime? ModerationReportDate { get; set; }

        [Display(Name = "Verification Trigger Date")]
        public DateTime? VerificationTriggerDate { get; set; }

        [Display(Name = "Reporting Intervals")]
        public string? ReportingIntervals { get; set; }

        [MaxLength(200), Display(Name = "Mentorship")]
        public string? Mentorship { get; set; }

        [Display(Name = "Soft Skills")]
        public string? SoftSkillsYN { get; set; }

        [MaxLength(200), Display(Name = "Add On")]
        public string? AnyAddOn { get; set; }

        [MaxLength(500), Display(Name = "Add On Comment")]
        public string? AddOnComment { get; set; }

        // Dropdowns
        public SelectList? Coordinators { get; set; }
        public SelectList? Facilitators { get; set; }
        public SelectList? TalentManagers { get; set; }
        public SelectList? PayrollUsers { get; set; }
        public SelectList? ReportingIntervalOptions { get; set; }
    }

    // ── Deliverable Create/Edit ──
    public class DeliverableViewModel
    {
        public int Id { get; set; }

        [Required]
        public int ProgrammeId { get; set; }
        public string? ProgrammeName { get; set; }
        public string? ClientName { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        [Display(Name = "Assigned To")]
        public int? AssignedToId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Notify Days Before")]
        [Range(1, 90)]
        public int NotifyDaysBefore { get; set; } = 7;

        public SelectList? Users { get; set; }
        public SelectList? Statuses { get; set; }
    }

    // ── User Create/Edit ──
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(50), Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50), Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string? Password { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public SelectList? Roles { get; set; }
        public int AssignedDeliverableCount { get; set; }
    }

    // ── SMTP Settings ──
    public class SmtpSettingsViewModel
    {
        [Required, Display(Name = "SMTP Host")]
        public string SmtpHost { get; set; } = string.Empty;

        [Required, Display(Name = "Port")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "Username")]
        public string? SmtpUsername { get; set; }

        [DataType(DataType.Password), Display(Name = "Password")]
        public string? SmtpPassword { get; set; }

        [Required, EmailAddress, Display(Name = "From Address")]
        public string SmtpFromAddress { get; set; } = string.Empty;

        [Display(Name = "Notifications Enabled")]
        public bool NotifyEnabled { get; set; } = true;

        [Display(Name = "Check Interval (seconds)")]
        [Range(60, 86400)]
        public int CheckIntervalSeconds { get; set; } = 3600;

        [EmailAddress, Display(Name = "Test Email Address")]
        public string? TestEmailAddress { get; set; }
    }
}

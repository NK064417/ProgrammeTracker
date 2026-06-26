using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgrammeTracker.Models
{
    public class Programme
    {
        public int Id { get; set; }

        // ── PHASE 1: CLIENT DETAILS ──
        [Required, MaxLength(20)]
        public string DeliveryType { get; set; } = string.Empty; // Campus | Online | Inhouse

        [Required, MaxLength(150)]
        public string ClientName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(100)]
        public string? ContactDetails { get; set; }

        public DateTime? StartDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Stipend { get; set; }

        [MaxLength(50)]
        public string? Bursary { get; set; }

        [Required, MaxLength(10)]
        public string Funding { get; set; } = "No"; // Yes | No

        [MaxLength(50)]
        public string? EEServices { get; set; }

        [MaxLength(50)]
        public string? WSPServices { get; set; }

        // ── PHASE 1: LEARNER DETAILS ──
        public int? NumberOfLearners { get; set; }

        [MaxLength(50)]
        public string? AbsorptionAssistance { get; set; } // Possible Absorption | No Absorption

        [MaxLength(20)]
        public string? DisabilityRatio { get; set; }

        [MaxLength(100)]
        public string? TypeOfDisability { get; set; }

        public int? MaleCapacity { get; set; }
        public int? FemaleCapacity { get; set; }
        public int? OtherGenderCapacity { get; set; }

        [MaxLength(200)]
        public string? EEComposition { get; set; }

        [MaxLength(50)]
        public string? PreferredProvince { get; set; }

        public int? AccountManagerId { get; set; }
        [ForeignKey(nameof(AccountManagerId))]
        public ApplicationUser? AccountManager { get; set; }

        // ── PHASE 1: SKILLS DEVELOPMENT ──
        [Required]
        public int ProgrammeTypeId { get; set; }
        [ForeignKey(nameof(ProgrammeTypeId))]
        public ProgrammeType? ProgrammeType { get; set; }

        [Required, MaxLength(200)]
        public string ProgrammeName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? TopUpSkillsTraining { get; set; }

        [MaxLength(5)]
        public string? SoftSkillsTraining { get; set; } // Yes | No

        [MaxLength(50)]
        public string? ReportingFrequency { get; set; }

        [MaxLength(500)]
        public string? LearnerRequiredAttributes { get; set; }

        // ── PHASE 1: PERIODS ──
        public int? TrainingPeriodMonths { get; set; }
        public int? HostingPeriodMonths { get; set; }
        public int? WorkplacePeriodMonths { get; set; }
        public int? TotalPeriodMonths { get; set; }

        // ── PHASE 1: ABSENTEEISM & REPLACEMENT ──
        [MaxLength(50)]
        public string? AbsenteeismManagedBy { get; set; } // DDNA | DDNA & Client

        [MaxLength(5)]
        public string? ReplacementPolicy { get; set; } // Y | N

        [MaxLength(50)]
        public string? ReplacementTimeframe { get; set; }

        // ── PHASE 2: INTERNAL IMPLEMENTATION ──
        public int? CoordinatorId { get; set; }
        [ForeignKey(nameof(CoordinatorId))]
        public ApplicationUser? Coordinator { get; set; }

        public int? FacilitatorId { get; set; }
        [ForeignKey(nameof(FacilitatorId))]
        public ApplicationUser? Facilitator { get; set; }

        public int? TalentManagerId { get; set; }
        [ForeignKey(nameof(TalentManagerId))]
        public ApplicationUser? TalentManager { get; set; }

        public int? PayrollId { get; set; }
        [ForeignKey(nameof(PayrollId))]
        public ApplicationUser? Payroll { get; set; }

        [MaxLength(100)]
        public string? SETAAdvisorName { get; set; }

        [MaxLength(100)]
        public string? SETAAdvisorContact { get; set; }

        public DateTime? LearnerPresentation1Date { get; set; }
        public DateTime? LearnerPresentation2Date { get; set; }
        public DateTime? LearnerRegistrationDeadline { get; set; }
        public DateTime? PlacementDate { get; set; }
        public DateTime? SoftSkillsStartDate { get; set; }
        public DateTime? ModerationReportDate { get; set; }
        public DateTime? VerificationTriggerDate { get; set; }

        [MaxLength(50)]
        public string? ReportingIntervals { get; set; }

        [MaxLength(200)]
        public string? Mentorship { get; set; }

        [MaxLength(5)]
        public string? SoftSkillsYN { get; set; }

        [MaxLength(200)]
        public string? AnyAddOn { get; set; }

        [MaxLength(500)]
        public string? AddOnComment { get; set; }

        // ── STATUS & AUDIT ──
        [Required, MaxLength(50)]
        public string Status { get; set; } = "Active"; // Active | Not Active | Completed

        public bool Phase1Complete { get; set; } = false;
        public bool Phase2Complete { get; set; } = false;
        public bool IsLegacyImport { get; set; } = false;

        [Required]
        public int CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();

        // Computed helpers
        public DateTime? EndDate => StartDate.HasValue && TotalPeriodMonths.HasValue
            ? StartDate.Value.AddMonths(TotalPeriodMonths.Value)
            : null;

        public void ComputePhaseCompletion()
        {
            Phase1Complete = !string.IsNullOrWhiteSpace(ClientName)
                && !string.IsNullOrWhiteSpace(DeliveryType)
                && !string.IsNullOrWhiteSpace(Funding)
                && ProgrammeTypeId > 0
                && !string.IsNullOrWhiteSpace(ProgrammeName);

            Phase2Complete = CoordinatorId.HasValue
                && FacilitatorId.HasValue
                && TalentManagerId.HasValue
                && PlacementDate.HasValue
                && LearnerPresentation1Date.HasValue;
        }
    }
}

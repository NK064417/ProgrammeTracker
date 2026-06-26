using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgrammeTracker.Models
{
    public class Deliverable
    {
        public int Id { get; set; }

        [Required]
        public int ProgrammeId { get; set; }
        [ForeignKey(nameof(ProgrammeId))]
        public Programme? Programme { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public int? AssignedToId { get; set; }
        [ForeignKey(nameof(AssignedToId))]
        public ApplicationUser? AssignedTo { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending | In Progress | Complete | Overdue

        public int NotifyDaysBefore { get; set; } = 7;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();

        // Computed helpers
        public int DaysRemaining => (DueDate.Date - DateTime.UtcNow.Date).Days;
        public bool IsOverdue => DaysRemaining < 0 && Status != "Complete";
        public bool IsDueSoon => DaysRemaining >= 0 && DaysRemaining <= NotifyDaysBefore && Status != "Complete";
    }
}

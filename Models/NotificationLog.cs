using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgrammeTracker.Models
{
    public class NotificationLog
    {
        public int Id { get; set; }

        [Required]
        public int DeliverableId { get; set; }
        [ForeignKey(nameof(DeliverableId))]
        public Deliverable? Deliverable { get; set; }

        public int? SentToUserId { get; set; }
        [ForeignKey(nameof(SentToUserId))]
        public ApplicationUser? SentToUser { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Sent"; // Sent | Failed

        [MaxLength(500)]
        public string? Message { get; set; }
    }

    public class AppSettings
    {
        [Key, MaxLength(100)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Value { get; set; }
    }
}

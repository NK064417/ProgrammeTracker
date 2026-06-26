using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProgrammeTracker.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Navigation
        public ICollection<Programme> ManagedProgrammes { get; set; } = new List<Programme>();
        public ICollection<Programme> CoordinatedProgrammes { get; set; } = new List<Programme>();
        public ICollection<Programme> FacilitatedProgrammes { get; set; } = new List<Programme>();
        public ICollection<Programme> TalentManagedProgrammes { get; set; } = new List<Programme>();
        public ICollection<Deliverable> AssignedDeliverables { get; set; } = new List<Deliverable>();
        public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
    }
}

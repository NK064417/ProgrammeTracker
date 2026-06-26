using System.ComponentModel.DataAnnotations;

namespace ProgrammeTracker.Models
{
    public class ProgrammeType
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public int? DefaultPeriodMonths { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Programme> Programmes { get; set; } = new List<Programme>();
    }
}

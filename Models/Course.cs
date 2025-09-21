using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public int InstructorId { get; set; }

        // Navigation properties
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
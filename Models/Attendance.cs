using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Session Session { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }

    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late
    }
}
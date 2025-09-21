using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AttendanceApi.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Dni { get; set; } = string.Empty;

        // Navigation properties
        [JsonIgnore]
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
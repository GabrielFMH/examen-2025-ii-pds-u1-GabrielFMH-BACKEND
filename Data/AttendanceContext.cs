using AttendanceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Data
{
    public class AttendanceContext : DbContext
    {
        public AttendanceContext(DbContextOptions<AttendanceContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.CourseId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Session)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.SessionId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId);

            // Seed data
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "Mathematics 101", Description = "Basic Mathematics", InstructorId = 1 },
                new Course { Id = 2, Name = "Physics 101", Description = "Basic Physics", InstructorId = 1 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
                new Student { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" }
            );
        }
    }
}
using AttendanceApi.Controllers;
using AttendanceApi.Data;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AttendanceApi.Tests
{
    public class ReportsControllerTests
    {
        private AttendanceContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AttendanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AttendanceContext(options);
        }

        [Fact]
        public void GetAttendanceReport_ReturnsReport()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var course = new Course { Id = 1, Name = "Test Course", Description = "Test", InstructorId = 1 };
            var session = new Session { Id = 1, CourseId = 1, Date = DateTime.UtcNow, Topic = "Test Session" };
            var student = new Student { Id = 1, Name = "Test Student", Email = "test@example.com", Dni = "12345678" };
            var attendance = new Attendance { Id = 1, SessionId = 1, StudentId = 1, Status = AttendanceStatus.Present, RecordedAt = DateTime.UtcNow };
            context.Courses.Add(course);
            context.Sessions.Add(session);
            context.Students.Add(student);
            context.Attendances.Add(attendance);
            context.SaveChanges();
            var controller = new ReportsController(context);

            // Act
            var result = controller.GetAttendanceReport();

            // Assert
            var actionResult = Assert.IsType<ActionResult<object>>(result);
            var report = Assert.IsAssignableFrom<object>(actionResult.Value);
            Assert.NotNull(report);
        }
    }
}
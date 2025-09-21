using AttendanceApi.Controllers;
using AttendanceApi.Data;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AttendanceApi.Tests
{
    public class AttendanceControllerTests
    {
        private AttendanceContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AttendanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AttendanceContext(options);
        }

        [Fact]
        public async Task GetAttendance_ReturnsAttendanceByCourse()
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
            var controller = new AttendanceController(context);

            // Act
            var result = await controller.GetAttendance(1, null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Attendance>>>(result);
            var attendances = Assert.IsAssignableFrom<IEnumerable<Attendance>>(actionResult.Value);
            Assert.Single(attendances);
        }

        [Fact]
        public async Task RegisterAttendance_CreatesNewStudentAndAttendance()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var session = new Session { Id = 1, CourseId = 1, Date = DateTime.UtcNow, Topic = "Test Session" };
            context.Sessions.Add(session);
            context.SaveChanges();
            var controller = new AttendanceController(context);
            var request = new RegisterAttendanceRequest { Name = "New Student", Dni = "12345678", SessionId = 1 };

            // Act
            var result = await controller.RegisterAttendance(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Single(context.Students);
            Assert.Single(context.Attendances);
            var student = context.Students.First();
            Assert.Equal("New Student", student.Name);
            Assert.Equal("12345678", student.Dni);
            var attendance = context.Attendances.First();
            Assert.Equal(AttendanceStatus.Present, attendance.Status);
        }
    }
}
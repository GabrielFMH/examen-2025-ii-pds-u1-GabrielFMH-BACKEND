using AttendanceApi.Controllers;
using AttendanceApi.Data;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AttendanceApi.Tests
{
    public class SessionsControllerTests
    {
        private AttendanceContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AttendanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AttendanceContext(options);
        }

        [Fact]
        public async Task GetSessions_ReturnsAllSessions()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Sessions.Add(new Session { Id = 1, CourseId = 1, Date = DateTime.UtcNow, Topic = "Test Session" });
            context.SaveChanges();
            var controller = new SessionsController(context);

            // Act
            var result = await controller.GetSessions();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Session>>>(result);
            var sessions = Assert.IsAssignableFrom<IEnumerable<Session>>(actionResult.Value);
            Assert.Single(sessions);
        }

        [Fact]
        public async Task PostSession_AddsSession()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new SessionsController(context);
            var newSession = new Session { CourseId = 1, Date = DateTime.UtcNow, Topic = "New Session" };

            // Act
            var result = await controller.PostSession(newSession);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Session>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var session = Assert.IsType<Session>(createdAtActionResult.Value);
            Assert.Equal("New Session", session.Topic);
        }

        [Fact]
        public async Task DeleteSession_RemovesSession()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var session = new Session { Id = 1, CourseId = 1, Date = DateTime.UtcNow, Topic = "Test Session" };
            context.Sessions.Add(session);
            context.SaveChanges();
            var controller = new SessionsController(context);

            // Act
            var result = await controller.DeleteSession(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Sessions);
        }
    }
}
using AttendanceApi.Controllers;
using AttendanceApi.Data;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AttendanceApi.Tests
{
    public class CoursesControllerTests
    {
        private AttendanceContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AttendanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AttendanceContext(options);
        }

        [Fact]
        public async Task GetCourses_ReturnsAllCourses()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Courses.Add(new Course { Id = 1, Name = "Test Course", Description = "Test", InstructorId = 1 });
            context.SaveChanges();
            var controller = new CoursesController(context);

            // Act
            var result = await controller.GetCourses();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Course>>>(result);
            var courses = Assert.IsAssignableFrom<IEnumerable<Course>>(actionResult.Value);
            Assert.Single(courses);
        }

        [Fact]
        public async Task PostCourse_AddsCourse()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new CoursesController(context);
            var newCourse = new Course { Name = "New Course", Description = "New", InstructorId = 1 };

            // Act
            var result = await controller.PostCourse(newCourse);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Course>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var course = Assert.IsType<Course>(createdAtActionResult.Value);
            Assert.Equal("New Course", course.Name);
        }

        [Fact]
        public async Task DeleteCourse_RemovesCourse()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var course = new Course { Id = 1, Name = "Test Course", Description = "Test", InstructorId = 1 };
            context.Courses.Add(course);
            context.SaveChanges();
            var controller = new CoursesController(context);

            // Act
            var result = await controller.DeleteCourse(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Courses);
        }
    }
}
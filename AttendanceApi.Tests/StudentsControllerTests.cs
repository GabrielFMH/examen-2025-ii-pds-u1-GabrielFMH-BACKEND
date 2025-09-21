using AttendanceApi.Controllers;
using AttendanceApi.Data;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AttendanceApi.Tests
{
    public class StudentsControllerTests
    {
        private AttendanceContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AttendanceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AttendanceContext(options);
        }

        [Fact]
        public async Task GetStudents_ReturnsAllStudents()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Students.Add(new Student { Id = 1, Name = "Test Student", Email = "test@example.com", Dni = "12345678" });
            context.SaveChanges();
            var controller = new StudentsController(context);

            // Act
            var result = await controller.GetStudents();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Student>>>(result);
            var students = Assert.IsAssignableFrom<IEnumerable<Student>>(actionResult.Value);
            Assert.Single(students);
        }

        [Fact]
        public async Task PostStudent_AddsStudent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new StudentsController(context);
            var newStudent = new Student { Name = "New Student", Email = "new@example.com", Dni = "87654321" };

            // Act
            var result = await controller.PostStudent(newStudent);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Student>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var student = Assert.IsType<Student>(createdAtActionResult.Value);
            Assert.Equal("New Student", student.Name);
        }

        [Fact]
        public async Task DeleteStudent_RemovesStudent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var student = new Student { Id = 1, Name = "Test Student", Email = "test@example.com", Dni = "12345678" };
            context.Students.Add(student);
            context.SaveChanges();
            var controller = new StudentsController(context);

            // Act
            var result = await controller.DeleteStudent(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Students);
        }
    }
}
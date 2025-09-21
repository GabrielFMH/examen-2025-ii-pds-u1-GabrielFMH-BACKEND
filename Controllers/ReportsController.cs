// ReportsController.cs
using AttendanceApi.Data;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Asegúrate de tener esta directiva

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public ReportsController(AttendanceContext context)
        {
            _context = context;
        }

        // GET: api/reports/attendance
        [HttpGet("attendance")]
        // Cambiamos el tipo de retorno ya que no es una operación asincrónica pura después de AsEnumerable()
        public ActionResult<object> GetAttendanceReport() 
        {
            try
            {
                // Ejecutamos la consulta en la base de datos y traemos los datos a memoria
                var attendancesWithRelatedData = _context.Attendances
                    .Include(a => a.Session)
                    .ThenInclude(s => s.Course)
                    .Include(a => a.Student)
                    .AsEnumerable() // <-- Esto ejecuta la consulta y trae los datos
                    .ToList(); // Materializamos la lista (aunque AsEnumerable ya trae los datos)

                // Ahora hacemos el GroupBy y Select en memoria (LINQ to Objects)
                var report = attendancesWithRelatedData
                    .GroupBy(a => new {
                        CourseId = a.Session?.CourseId ?? 0,
                        CourseName = a.Session?.Course?.Name ?? "Unknown Course",
                        StudentId = a.StudentId,
                        StudentName = a.Student?.Name ?? "Unknown Student"
                    })
                    .Select(g => new
                    {
                        CourseId = g.Key.CourseId,
                        CourseName = g.Key.CourseName,
                        StudentId = g.Key.StudentId,
                        StudentName = g.Key.StudentName,
                        TotalSessions = g.Count(),
                        PresentCount = g.Count(a => a.Status == AttendanceStatus.Present),
                        AbsentCount = g.Count(a => a.Status == AttendanceStatus.Absent),
                        LateCount = g.Count(a => a.Status == AttendanceStatus.Late),
                        AttendancePercentage = g.Count() > 0 ? 
                            (double)g.Count(a => a.Status == AttendanceStatus.Present) / g.Count() * 100 : 0
                    })
                    .ToList(); // Materializar el resultado final

                return Ok(report); // Devolvemos el resultado usando Ok()
            }
            catch (Exception ex)
            {
                // Manejo básico de errores
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
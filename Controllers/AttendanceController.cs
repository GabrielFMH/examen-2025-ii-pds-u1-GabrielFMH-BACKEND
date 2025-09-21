using AttendanceApi.Data;
using AttendanceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Controllers
{
    public class RegisterAttendanceRequest
    {
        public required string Name { get; set; }
        public required string Dni { get; set; }
        public int SessionId { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public AttendanceController(AttendanceContext context)
        {
            _context = context;
        }

        // GET: api/attendance?courseId=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance([FromQuery] int? courseId, [FromQuery] int? studentId)
        {
            IQueryable<Attendance> query = _context.Attendances
                .Include(a => a.Session)
                .Include(a => a.Student);

            if (courseId.HasValue)
            {
                query = query.Where(a => a.Session != null && a.Session.CourseId == courseId.Value);
            }

            if (studentId.HasValue)
            {
                query = query.Where(a => a.StudentId == studentId.Value);
            }

            return await query.ToListAsync();
        }

        // POST: api/attendance
        [HttpPost]
        public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttendance", new { id = attendance.Id }, attendance);
        }

        // PUT: api/attendance/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int id, Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return BadRequest();
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/attendance/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAttendance([FromBody] RegisterAttendanceRequest request)
        {
            // Find or create student
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Dni == request.Dni);
            if (student == null)
            {
                student = new Student { Name = request.Name, Dni = request.Dni, Email = $"{request.Name.ToLower().Replace(" ", "")}@example.com" };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }

            // Check if attendance already exists
            var existingAttendance = await _context.Attendances.FirstOrDefaultAsync(a => a.SessionId == request.SessionId && a.StudentId == student.Id);
            if (existingAttendance != null)
            {
                return BadRequest("Ya has registrado asistencia para esta sesión.");
            }

            // Create attendance
            var attendance = new Attendance
            {
                SessionId = request.SessionId,
                StudentId = student.Id,
                Status = AttendanceStatus.Present,
                RecordedAt = DateTime.UtcNow
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok("Asistencia registrada exitosamente.");
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.Id == id);
        }
    }
}
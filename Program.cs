using AttendanceApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Add DbContext
builder.Services.AddDbContext<AttendanceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AttendanceContext>();
    db.Database.Migrate();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // OpenAPI/Swagger no disponible en .NET 8.0 en producción
}

// ✅ ✅ ✅ AÑADE ESTA LÍNEA: Usa el puerto que Railway asigna automáticamente
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var uri = $"http://0.0.0.0:{port}";
builder.WebHost.UseUrls(uri);

app.MapGet("/", () => "Hola desde AttendanceApi");

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
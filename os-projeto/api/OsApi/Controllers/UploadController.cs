using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OsApi.Data;
using OsApi.Domain;

namespace OsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("os/{id:int}/foto")]
    public class UploadController : ControllerBase
    {
        private readonly OsDbContext _db;
        private readonly IWebHostEnvironment _env;

        public UploadController(OsDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost]
        [RequestSizeLimit(5_000_00)]
        public async Task<IActionResult> Upload([FromRoute] int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Arquivo inválido");
            }

            var os = await _db.OS.FirstOrDefaultAsync(o => o.Id == id);
            if (os == null)
            {
                return NotFound("Ordem de serviço não encontrada");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadDir = Path.Combine(webRoot, "uploads");
            Directory.CreateDirectory(uploadDir);

            var safeName = Path.GetFileName(file.FileName);
            var unique = $"{Guid.NewGuid()}_{safeName}";
            var fullPath = Path.Combine(uploadDir, unique);

            await using (var fs = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(fs);
            }

            var foto = new OSFoto
            {
                OsId = os.Id,
                Path = $"/uploads/{unique}",
                UploadedAt = DateTime.UtcNow
            };

            _db.OSFotos.Add(foto);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                foto.Id,
                foto.Path,
                foto.UploadedAt
            });
        }
    }
}
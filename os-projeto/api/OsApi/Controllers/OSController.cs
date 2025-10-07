using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OsApi.Contracts;
using OsApi.Data;
using OsApi.Domain;

namespace OsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("os")]
    public class OSController : ControllerBase
    {
        private readonly OsDbContext _db;
        public OSController(OsDbContext db)
        {
            _db = db;
        }

        private int? UserId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            return int.TryParse(sub, out var idOk) ? idOk : null;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOsReq req)
        {
            if (string.IsNullOrWhiteSpace(req.Titulo))
            {
                return BadRequest("O titulo é obrigatorio");
            }

            int? userId = UserId();

            if (userId is null)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;

                if (!string.IsNullOrWhiteSpace(email))
                {
                    var u = await _db.Users.AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Email == email);
                    if (u != null) userId = u.Id;
                }
            }

            if (userId is null)
            {
                return Unauthorized("Não foi possível indetificar o usuário");
            }

            var os = new OrdemServico
            {
                Titulo = req.Titulo.Trim(),
                Descricao = string.IsNullOrWhiteSpace(req.Descricao) ? null : req.Descricao,
                TecnicoId = userId,
                Status = StatusOS.Aberta,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.OS.Add(os);
            await _db.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetById), new
                {
                    id = os.Id
                },
                new
                {
                    os.Id
                }
            );
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] StatusOS? statusm,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int size = 20)
        {
            if (page < 1) page = 1;
            if (size < 1 || size > 100) size = 20;

            var q = _db.OS.AsNoTracking().Include(o => o.Tecnico).AsQueryable();

            int? userId = UserId();

            if (userId.HasValue && !User.IsInRole("Admin"))
            {
                q = q.Where(o => o.TecnicoId == userId.Value);
            }


            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(o =>
                        o.Titulo.Contains(s) ||
                        (o.Descricao != null && o.Descricao.Contains(s)));
            }

            var total = await q.CountAsync();
            var items = await q
                .OrderByDescending(o => o.UpdatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(o => new
                {
                    o.Id,
                    o.Titulo,
                    o.Status,
                    o.TecnicoId,
                    Tecnico = o.Tecnico != null ? new { o.Tecnico.Id, o.Tecnico.Nome, o.Tecnico.Email } : null,
                    o.CreatedAt,
                    o.UpdatedAt
                })
                .ToListAsync();
            return Ok(new { total, page, size, items });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var os = await _db.OS
                .AsNoTracking()
                .Include(o => o.Tecnico)
                .Include(o => o.Fotos)
                .Include(o => o.Checks)
                    .ThenInclude(c => c.Item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (os == null) return NotFound();

            return Ok(new
            {
                os.Id,
                os.Titulo,
                os.Descricao,
                os.Status,
                os.TecnicoId,
                Tecnico = os.Tecnico != null ? new { os.Tecnico.Id, os.Tecnico.Nome, os.Tecnico.Email } : null,
                os.CreatedAt,
                os.UpdatedAt,
                Fotos = os.Fotos.Select(f => new { f.Id, f.Path, f.UploadedAt, f.ChecklistItemId }),
                Checks = os.Checks.Select(c => new
                {
                    c.ChecklistItemId,
                    Item = new { c.Item.Id, c.Item.Titulo, c.Item.Tipo, c.Item.Obrigatorio },
                    c.Marcado,
                    c.ValorNumero,
                    c.ValorTexto,
                    c.ObrigatorioSnapshot
                })
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateOsReq req)
        {
            var os = await _db.OS.FirstOrDefaultAsync(o => o.Id == id);
            if (os == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(req.Titulo)) os.Titulo = req.Titulo.Trim();
            if (req.Descricao != null) os.Descricao = string.IsNullOrWhiteSpace(req.Descricao) ? null : req.Descricao.Trim();
            if (req.TecnicoId.HasValue) os.TecnicoId = req.TecnicoId.Value;
            if (req.Status.HasValue) os.Status = req.Status.Value;


            os.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromBody] UpdateOsReq req)
        {
            var os = await _db.OS
                .Include(o => o.Fotos)
                .Include(o => o.Checks)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (os == null) return NotFound("Ordem de serviço não encontrada.");

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1) Apagar Sarquivos físicos das fotos (quando forem locais em /wwwroot/uploads)
                var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                foreach (var f in os.Fotos)
                {
                    var p = f.Path?.Trim() ?? string.Empty;

                    // pula URLs absolutas (CDN etc.)
                    var isUrl = p.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                             || p.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
                    if (isUrl) continue;

                    // normaliza caminho relativo tipo "/uploads/xyz.jpg"
                    var rel = p.Replace('\\', '/').TrimStart('/'); // -> "uploads/xyz.jpg"
                    var full = Path.Combine(webRoot, rel);

                    try
                    {
                        if (System.IO.File.Exists(full))
                            System.IO.File.Delete(full);
                    }
                    catch
                    {
                    }
                }

                // 2) Remover dependências no banco
                if (os.Fotos.Any())
                    _db.OSFotos.RemoveRange(os.Fotos);

                if (os.Checks.Any())
                    _db.OSChecklists.RemoveRange(os.Checks);

                // 3) Remover a OS
                _db.OS.Remove(os);

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return NoContent();
            }
            catch
            {
                await tx.RollbackAsync();
                return StatusCode(500, "Falha ao remover a OS e seus anexos.");
            }
        }

    }

}
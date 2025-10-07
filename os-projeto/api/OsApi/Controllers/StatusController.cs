using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OsApi.Data;
using OsApi.Domain;

namespace OsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("os/{id:int}/status")]
    public class OsStatusController : ControllerBase
    {
        private readonly OsDbContext _db;
        public OsStatusController(OsDbContext db) => _db = db;

        private int? UserId()
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            return int.TryParse(sub, out var idOk) ? idOk : null;
        }

        private async Task<string?> ValidarOs(OrdemServico os)
        {
            var obrigatorios = await _db.ChecklistItems
                    .Where(ci => ci.Ativo && ci.Obrigatorio)
                    .Select(ci => new { ci.Id, ci.Tipo })
                    .ToListAsync();

            var checksPorItem = os.Checks.ToDictionary(c => c.ChecklistItemId, c => c);

            var faltantes = obrigatorios
                .Where(ob =>
                    !checksPorItem.ContainsKey(ob.Id) &&
                    !(ob.Tipo == TipoChecklist.Foto && os.Fotos.Any(f => f.ChecklistItemId == ob.Id))
                )
                .Select(ob => ob.Id)
                .ToList();

            if (faltantes.Any())
                return $"Itens obrigatórios sem marcação: {string.Join(", ", faltantes)}";


            var idsMarcados = os.Checks.Select(c => c.ChecklistItemId).Distinct().ToList();
            var metas = await _db.ChecklistItems
                .Where(ci => idsMarcados.Contains(ci.Id))
                .ToDictionaryAsync(ci => ci.Id);

            foreach (var c in os.Checks)
            {
                if (c.ObrigatorioSnapshot != true) continue;
                if (!metas.TryGetValue(c.ChecklistItemId, out var meta)) continue;

                switch (meta.Tipo)
                {
                    case TipoChecklist.Boolean:
                        if (c.Marcado is null)
                            return $"Item {c.ChecklistItemId} (Boolean) obrigatório sem 'marcado'.";
                        break;

                    case TipoChecklist.Numero:
                        if (c.ValorNumero is null)
                            return $"Item {c.ChecklistItemId} (Numero) obrigatório sem 'valorNumero'.";
                        break;

                    case TipoChecklist.Texto:
                        if (string.IsNullOrWhiteSpace(c.ValorTexto))
                            return $"Item {c.ChecklistItemId} (Texto) obrigatório sem 'valorTexto'.";
                        break;

                    case TipoChecklist.Foto:
                        var temFoto = os.Fotos.Any(f => f.ChecklistItemId == c.ChecklistItemId);
                        if (!temFoto)
                            return $"Item {c.ChecklistItemId} (Foto) obrigatório requer ao menos 1 upload.";
                        break;
                }
            }


            return null;
        }

        [HttpPost("iniciar")]
        public async Task<IActionResult> Iniciar([FromRoute] int id)
        {
            var os = await _db.OS.FirstOrDefaultAsync(o => o.Id == id);
            if (os == null) return NotFound();

            if (os.Status != StatusOS.Aberta)
                return BadRequest("Somente OS 'Aberta' pode ser iniciada.");

            var uid = UserId();
            if (uid is null) return Unauthorized("Usuário não identificado.");

            if (os.TecnicoId == null) os.TecnicoId = uid;
            os.Status = StatusOS.EmAndamento;
            os.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("fechar")]
        public async Task<IActionResult> Fechar([FromRoute] int id)
        {
            var os = await _db.OS
                .Include(o => o.Checks)
                .Include(o => o.Fotos)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (os == null) return NotFound();

            if (os.Status != StatusOS.EmAndamento)
                return BadRequest("Somente OS 'EmAndamento' pode ser fechada.");

            var erro = await ValidarOs(os);
            if (erro is not null) return BadRequest(erro);

            os.Status = StatusOS.Fechada;
            os.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("reabrir")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reabrir([FromRoute] int id)
        {
            var os = await _db.OS.FirstOrDefaultAsync(o => o.Id == id);
            if (os == null) return NotFound();

            if (os.Status != StatusOS.Fechada)
                return BadRequest("Somente OS 'Fechada' pode ser reaberta.");

            os.Status = StatusOS.EmAndamento;
            os.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

    }
}
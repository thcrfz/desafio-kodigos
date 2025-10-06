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
    public class ChecklistController : ControllerBase
    {
        private readonly OsDbContext _db;
        public ChecklistController(OsDbContext db)
        {
            _db = db;
        }

        [HttpGet("checklist")]
        public async Task<IActionResult> GetCatalog()
        {
            var items = await _db.ChecklistItems
                .AsNoTracking()
                .Where(x => x.Ativo)
                .OrderBy(x => x.Ordem)
                .Select(x => new
                {
                    x.Id,
                    x.Titulo,
                    x.Tipo,
                    x.Obrigatorio,
                    x.Ordem
                })
                .ToListAsync();
            return Ok(items);
        }

        [HttpPost("os/{id:int}/checklist")]
        public async Task<IActionResult> UpsertMarks([FromRoute] int id, [FromBody] List<OsChecklistMarkReq> body)
        {
            if (body == null || body.Count == 0)
            {
                return BadRequest("Lista vazia.");
            }

            var os = await _db.OS
                .Include(o => o.Checks)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (os == null)
            {
                return NotFound("Ordem de serviço não encontrada");
            }
            // Pega uma lista de ids
            var itemsIds = body.Select(b => b.ItemId).Distinct().ToList();
            // Busca items que contem os ids da lista
            var items = await _db.ChecklistItems
                .Where(ci => itemsIds.Contains(ci.Id))
                .ToDictionaryAsync(ci => ci.Id);

            foreach (var req in body)
            {
                if (!items.TryGetValue(req.ItemId, out var meta))
                {
                    return BadRequest($"ChecklistItem {req.ItemId} inexistente.");
                }

                switch (meta.Tipo)
                {
                    case TipoChecklist.Boolean:
                        if (req.Marcado is null)
                        {
                            return BadRequest($"Item {req.ItemId} requer 'marcado'.");
                        }
                        break;
                    case TipoChecklist.Numero:
                        if (req.ValorNumero is null)
                        {
                            return BadRequest($"Item {req.ItemId} requer 'valorNumero'.");
                        }
                        break;

                    case TipoChecklist.Texto:
                        if (req.ValorTexto is null)
                        {
                            return BadRequest($"Item {req.ItemId} requer 'valorTexto'.");
                        }
                        break;
                }

                var existing = os.Checks.FirstOrDefault(c => c.ChecklistItemId == req.ItemId);

                if (existing == null)
                {
                    existing = new OSChecklist
                    {
                        OsId = os.Id,
                        ChecklistItemId = req.ItemId,
                        ObrigatorioSnapshot = meta.Obrigatorio
                    };
                    os.Checks.Add(existing);
                    existing = SetExistingValues(existing, meta, req);
                }
            }
            await _db.SaveChangesAsync();
            return NoContent();
        }

        private static OSChecklist SetExistingValues(OSChecklist oSChecklist, ChecklistItem meta, OsChecklistMarkReq req)
        {
            oSChecklist.Marcado = meta.Tipo == TipoChecklist.Boolean ? req.Marcado : null;
            oSChecklist.ValorNumero = meta.Tipo == TipoChecklist.Numero ? req.ValorNumero : null;
            oSChecklist.ValorTexto = meta.Tipo == TipoChecklist.Texto ? (req.ValorTexto ?? string.Empty) : null;
            return oSChecklist;
        }
    }
}
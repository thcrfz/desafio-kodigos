using OsApi.Domain;

namespace OsApi.Data;

public static class ChecklistSeed
{
    public static void Run(OsDbContext db)
    {
        if (db.ChecklistItems.Any()) return;

        db.ChecklistItems.AddRange(new[]
        {
            new ChecklistItem { Titulo = "Confirma uso de EPI?", Tipo = TipoChecklist.Boolean, Obrigatorio = true,  Ativo = true, Ordem = 1 },
            new ChecklistItem { Titulo = "Tensão medida (V)",    Tipo = TipoChecklist.Numero,  Obrigatorio = false, Ativo = true, Ordem = 2 },
            new ChecklistItem { Titulo = "Observações",          Tipo = TipoChecklist.Texto,   Obrigatorio = false, Ativo = true, Ordem = 3 },
            new ChecklistItem { Titulo = "Foto antes",           Tipo = TipoChecklist.Foto,    Obrigatorio = true,  Ativo = true, Ordem = 4 },
            new ChecklistItem { Titulo = "Foto depois",          Tipo = TipoChecklist.Foto,    Obrigatorio = true,  Ativo = true, Ordem = 5 },
        });

        db.SaveChanges();
    }
}

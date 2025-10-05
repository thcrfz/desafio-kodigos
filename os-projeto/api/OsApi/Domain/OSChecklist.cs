namespace OsApi.Domain
{
    public class OSChecklist
    {
        public int OsId { get; set; }
        public OrdemServico OS { get; set; } = default!;

        public int ChecklistItemId { get; set; }
        public ChecklistItem Item { get; set; } = default!;

        public bool? Marcado { get; set; }
        public double? ValorNumero { get; set; }
        public string? ValorTexto { get; set; }

        public bool? ObrigatorioSnapshot { get; set; }
    }
}

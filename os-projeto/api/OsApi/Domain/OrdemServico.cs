namespace OsApi.Domain
{
    public enum StatusOS { Aberta = 0, EmAndamento = 1, Fechada = 2 }

    public class OrdemServico
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = default!;
        public string? Descricao { get; set; }

        public StatusOS Status { get; set; } = StatusOS.Aberta;

        public int? TecnicoId { get; set; }
        public User? Tecnico { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<OSChecklist> Checks { get; set; } = new();
        public List<OSFoto> Fotos { get; set; } = new();
    }
}
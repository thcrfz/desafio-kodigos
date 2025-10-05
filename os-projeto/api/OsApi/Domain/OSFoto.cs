namespace OsApi.Domain
{
    public class OSFoto
    {
        public int Id { get; set; }

        public int OsId { get; set; }
        public OrdemServico OS { get; set; } = default!;

        public string Path { get; set; } = default!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

namespace OsApi.Domain
{
    public enum TipoChecklist { Boolean = 0, Numero = 1, Texto = 2, Foto = 3 }

    public class ChecklistItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = default!;
        public TipoChecklist Tipo { get; set; }
        public bool Obrigatorio { get; set; }
        public bool Ativo { get; set; } = true;
        public int Ordem { get; set; } = 0;
    }
}

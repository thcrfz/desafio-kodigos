namespace OsApi.Contracts
{
    public record CreateOsReq(string Titulo, string? Descricao);
    public record UpdateOsReq(string? Titulo, string? Descricao, int? TecnicoId, OsApi.Domain.StatusOS? Status);
}
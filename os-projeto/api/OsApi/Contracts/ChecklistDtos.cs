namespace OsApi.Contracts
{
    public record OsChecklistMarkReq(int ItemId, bool? Marcado, double? ValorNumero, string? ValorTexto);
}
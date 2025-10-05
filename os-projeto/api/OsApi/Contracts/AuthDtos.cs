namespace OsApi.Contracts
{
    public record RegisterReq(string Email, string Nome, string Password);
    public record LoginReq(string Email, string Password);
}
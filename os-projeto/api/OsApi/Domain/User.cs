using OsApi.Domain.Enums;

namespace OsApi.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Nome { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserRole UserRole { get; set; } = UserRole.Tecnico;
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OsApi.Contracts;
using OsApi.Data;
using OsApi.Domain;
using OsApi.Domain.Enums;

namespace OsApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly OsDbContext _db;
        private readonly IConfiguration _cfg;

        public AuthController(OsDbContext db, IConfiguration cfg)
        {
            _db = db; _cfg = cfg;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterReq req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Nome) ||
                string.IsNullOrWhiteSpace(req.Password))
            {
                return BadRequest("Campos obrigatórios.");
            }

            var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);

            if (exists) return Conflict("Email já existe");

            var user = new User
            {
                Email = req.Email.Trim(),
                Nome = req.Nome.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                UserRole = UserRole.Tecnico
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { user.Id, user.Email, user.Nome, user.CreatedAt });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReq req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null) return Unauthorized("Credenciais inválidas");

            var pass = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
            if (!pass) return Unauthorized("Credencias inválidas");

            var token = GenerateJwt(user);
            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.Nome,
                    user.UserRole
                }
            });
        }

        private string GenerateJwt(User user)
        {
            var issuer = _cfg["Jwt:Issuer"];
            var audience = _cfg["Jwt:Audience"];
            var key = _cfg["Jwt:Key"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
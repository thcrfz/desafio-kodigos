using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OsApi.Data;
using OsApi.Domain;
using OsApi.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OsDbContext>(opt =>
    opt.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        new MySqlServerVersion(new Version(8, 0, 0))
    ));

// CORS
builder.Services.AddCors(o => o.AddPolicy("web", p => p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
//JWT
builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(opt =>
  {
      opt.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
          RoleClaimType = ClaimTypes.Role
      };
  });

builder.Services.AddAuthorization();

// ADD CONTROLLERS
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "OsApi", Version = "v1" });

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "JWT no header. Use apenas o token aqui no Authorize.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        c.AddSecurityDefinition("Bearer", securityScheme);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OsDbContext>();
    db.Database.Migrate();
    ChecklistSeed.Run(db);

    // SEED ADMIN
    if (!await db.Users.AnyAsync(u => u.UserRole == UserRole.Admin))
    {
        var admin = new User
        {
            Email = "admin@local",
            Nome = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            UserRole = UserRole.Admin
        };
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}

app.UseStaticFiles();
app.UseCors("web");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

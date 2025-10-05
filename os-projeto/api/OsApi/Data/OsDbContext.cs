using Microsoft.EntityFrameworkCore;
using OsApi.Domain;

namespace OsApi.Data
{
    public class OsDbContext : DbContext
    {
        public OsDbContext(DbContextOptions<OsDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<OrdemServico> OS => Set<OrdemServico>();
        public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
        public DbSet<OSChecklist> OSChecklists => Set<OSChecklist>();
        public DbSet<OSFoto> OSFotos => Set<OSFoto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            // Os
            modelBuilder.Entity<OrdemServico>()
                .HasOne(o => o.Tecnico)
                .WithMany()
                .HasForeignKey(o => o.TecnincoId)
                .OnDelete(DeleteBehavior.Restrict);
            // Os Checklist
            modelBuilder.Entity<OSChecklist>()
                .HasOne(x => x.OS)
                .WithMany(o => o.Checks)
                .HasForeignKey(x => x.OsId);

            modelBuilder.Entity<OSChecklist>()
                .HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ChecklistItemId);

            // Indice para cada ativo
            modelBuilder.Entity<ChecklistItem>()
                .HasIndex(ci => ci.Ativo);

            base.OnModelCreating(modelBuilder);
        }
    }
}
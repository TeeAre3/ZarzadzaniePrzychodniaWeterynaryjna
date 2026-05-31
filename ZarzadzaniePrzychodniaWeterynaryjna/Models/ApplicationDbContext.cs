using Microsoft.EntityFrameworkCore;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Wlasciciel> Wlasciciele { get; set; } = null!;
        public DbSet<Zwierze> Zwierzeta { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=ZarzadzaniePrzychodniaWeterynaryjna;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wlasciciel>(entity =>
            {
                entity.HasIndex(w => w.Telefon).IsUnique();
                entity.HasIndex(w => w.Email).IsUnique();

                entity.HasCheckConstraint("CHK_Wlasciciel_Dane",
                    "([Imię] IS NOT NULL AND [Nazwisko] IS NOT NULL) OR [Nazwa_firmy] IS NOT NULL");
            });

            modelBuilder.Entity<Zwierze>(entity =>
            {
                entity.Property(z => z.Waga).HasColumnType("decimal(5,2)");

                entity.HasCheckConstraint("CHK_Zwierze_Waga", "[waga] > 0");
            });
        }
    }
}
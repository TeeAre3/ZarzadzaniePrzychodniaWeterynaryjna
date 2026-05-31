using Microsoft.EntityFrameworkCore;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Wlasciciel> Wlasciciele { get; set; } = null!;
        public DbSet<Zwierze> Zwierzeta { get; set; } = null!;
        public DbSet<Katalog> Katalogi { get; set; } = null!;
        public DbSet<Harmonogram> Harmonogramy { get; set; } = null!;
        public DbSet<WizytaMedyczna> Wizyty { get; set; } = null!;
        public DbSet<PozycjaWizyty> PozycjeWizyt { get; set; } = null!;

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

            modelBuilder.Entity<Katalog>(entity =>
            {
                entity.Property(k => k.CenaEwidencyjna).HasColumnType("decimal(8,2)");

                entity.Property(k => k.VAT).HasColumnType("decimal(5,2)");

                entity.HasCheckConstraint("CHK_Katalog_Cena", "[Cena_Ewidencyjna] >= 0");
            });

            modelBuilder.Entity<Harmonogram>(entity =>
            {
                entity.ToTable(tb => tb.HasTrigger("trg_BlokadaKonfliktowCzasowych"));

                entity.HasCheckConstraint("CHK_Czas_Rzeczywisty", "[Rzeczywisty_Czas_Rozpoczęcia] <= CURRENT_TIMESTAMP");
                entity.HasCheckConstraint("CHK_Czas_Zakon", "[Rzeczywisty_Czas_Zakończenia] >= [Rzeczywisty_Czas_Rozpoczęcia]");
            });

            modelBuilder.Entity<PozycjaWizyty>(entity =>
            {
                entity.HasCheckConstraint("CHK_PozycjaWizyty_Ilosc", "[Ilosc] > 0");
                entity.HasCheckConstraint("CHK_PozycjaWizyty_Cena", "[Cena_Zastosowana] >= 0");
            });
        }
    }
}
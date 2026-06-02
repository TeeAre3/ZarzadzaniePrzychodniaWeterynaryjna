using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.Configuration;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Data
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
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                string? connectionString = configuration.GetConnectionString("DefaultConnection")!;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wlasciciel>(entity =>
            {
                entity.HasIndex(w => w.Telefon).IsUnique();
                entity.HasIndex(w => w.Email).IsUnique();

                entity.ToTable(tb => tb.HasCheckConstraint("CHK_Wlasciciel_Dane",
                    "([Imię] IS NOT NULL AND [Nazwisko] IS NOT NULL) OR [Nazwa_firmy] IS NOT NULL"));
            });

            modelBuilder.Entity<Zwierze>(entity =>
            {
                entity.Property(z => z.Waga).HasColumnType("decimal(5,2)");
                entity.ToTable(tb => tb.HasCheckConstraint("CHK_Zwierze_Waga", "[waga] > 0"));
            });

            modelBuilder.Entity<Katalog>(entity =>
            {
                entity.Property(k => k.CenaEwidencyjna).HasColumnType("decimal(8,2)");
                entity.Property(k => k.VAT).HasColumnType("decimal(5,2)");
                entity.ToTable(tb => tb.HasCheckConstraint("CHK_Katalog_Cena", "[Cena_Ewidencyjna] >= 0"));
            });

            modelBuilder.Entity<Harmonogram>(entity =>
            {
                entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_BlokadaKonfliktowCzasowych");
                    tb.HasCheckConstraint("CHK_Czas_Rzeczywisty", "[Rzeczywisty_Czas_Rozpoczęcia] <= CURRENT_TIMESTAMP");
                    tb.HasCheckConstraint("CHK_Czas_Zakon", "[Rzeczywisty_Czas_Zakończenia] >= [Rzeczywisty_Czas_Rozpoczęcia]");
                });
            });

            modelBuilder.Entity<PozycjaWizyty>(entity =>
            {
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CHK_PozycjaWizyty_Ilosc", "[Ilosc] > 0");
                    tb.HasCheckConstraint("CHK_PozycjaWizyty_Cena", "[Cena_Zastosowana] >= 0");
                });
            });
        }
    }
}
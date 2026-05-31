using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Zwierzę")]
    public class Zwierze
    {
        [Key]
        [Column("id_zwierzęcia")]
        public int Id { get; set; }

        [Required]
        [Column("id_właściciela")]
        public int WlascicielId { get; set; }

        [Required]
        [Column("Imię")]
        [MaxLength(50)]
        public string Imie { get; set; } = string.Empty;

        [Required]
        [Column("Gatunek")]
        [MaxLength(50)]
        public string Gatunek { get; set; } = string.Empty;

        [Column("Rasa")]
        [MaxLength(50)]
        public string? Rasa { get; set; }

        [Required]
        [Column("Płeć")]
        [MaxLength(1)]
        public string Plec { get; set; } = string.Empty;

        [Column("Data_Urodzenia")]
        public DateTime? DataUrodzenia { get; set; }

        [Column("waga")]
        public decimal? Waga { get; set; }

        [ForeignKey(nameof(WlascicielId))]
        public Wlasciciel Wlasciciel { get; set; } = null!;

        public virtual ICollection<WizytaMedyczna> HistorieWizyt { get; set; } = new List<WizytaMedyczna>();
    }

}
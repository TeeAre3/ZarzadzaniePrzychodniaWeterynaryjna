using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Harmonogram")]
    public class Harmonogram
    {
        [Key]
        [Column("id_rezerwacji")]
        public int Id { get; set; }

        [Column("id_zwierzęcia")]
        public int ZwierzeId { get; set; }

        [ForeignKey("ZwierzeId")]
        public virtual Zwierze? Zwierze { get; set; }

        [Required]
        [Column("Planowana_Data_Rozpoczęcia")]
        public DateTime PlanowanaDataRozpoczecia { get; set; }

        [Column("Szacowany_czas_trwania")]
        public int SzacowanyCzasTrwania { get; set; }

        [Column("Powód_Wizyty")]
        [MaxLength(150)]
        public string? PowodWizyty { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Status_Wizyty")]
        public string StatusWizyty { get; set; } = "Planowana"; 

        [Column("Rzeczywisty_Czas_Rozpoczęcia")]
        public DateTime? RzeczywistyCzasRozpoczecia { get; set; }

        [Column("Rzeczywisty_Czas_Zakończenia")]
        public DateTime? RzeczywistyCzasZakonczenia { get; set; }
    }
}
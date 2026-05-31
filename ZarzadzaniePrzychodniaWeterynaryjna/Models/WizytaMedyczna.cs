using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Wizyta_Medyczna")]
    public class WizytaMedyczna
    {
        [Key]
        [Column("id_wizyty")]
        public int Id { get; set; }

        [Column("id_rezerwacji")]
        public int RezerwacjaId { get; set; }

        [ForeignKey("RezerwacjaId")]
        public virtual Harmonogram? Rezerwacja { get; set; }

        [Required]
        [Column("Data_Wizyty")]
        public DateTime DataWizyty { get; set; }

        [Column("Opis_Wywiadu")]
        public string? OpisWywiadu { get; set; }

        [Column("Rozpoznanie")]
        public string? Rozpoznanie { get; set; }

        [Column("Zalecenia")]
        public string? Zalecenia { get; set; }
    }
}
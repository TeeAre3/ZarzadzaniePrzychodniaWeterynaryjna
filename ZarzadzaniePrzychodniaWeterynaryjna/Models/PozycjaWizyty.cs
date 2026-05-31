using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Pozycja_Wizyty")]
    public class PozycjaWizyty
    {
        [Key]
        [Column("id_pozycji_wizyty")]
        public int Id { get; set; }

        [Required]
        [Column("id_wizyty")]
        public int WizytaId { get; set; }

        [ForeignKey(nameof(WizytaId))]
        public virtual WizytaMedyczna Wizyta { get; set; } = null!;

        [Required]
        [Column("id_katalogu")]
        public int KatalogId { get; set; }

        [ForeignKey(nameof(KatalogId))]
        public virtual Katalog Katalog { get; set; } = null!;

        [Required]
        [Column("Ilosc", TypeName = "decimal(8,2)")]
        public decimal Ilosc { get; set; } = 1;

        [Required]
        [Column("Cena_Zastosowana", TypeName = "decimal(8,2)")]
        public decimal CenaZastosowana { get; set; }

        [Required]
        [Column("VAT_Zastosowany", TypeName = "decimal(5,2)")]
        public decimal VATZastosowany { get; set; }
    }
}
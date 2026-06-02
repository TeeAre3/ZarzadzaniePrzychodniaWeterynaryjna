using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Katalog")]
    public class CatalogItem
    {
        [Key]
        [Column("id_pozycji")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Nazwa")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("Typ")]
        public string ItemType { get; set; } = string.Empty; 

        [Column("Cena_Ewidencyjna")]
        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        [Required]
        [Column("Jednostka_Miary")]
        [MaxLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Column("czas_trwania_w_min")]
        public int DurationMin { get; set; }
    }
}
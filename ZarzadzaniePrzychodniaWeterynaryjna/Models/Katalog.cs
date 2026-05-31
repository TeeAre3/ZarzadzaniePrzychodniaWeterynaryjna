using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Katalog")]
    public class Katalog
    {
        [Key]
        [Column("id_pozycji")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Typ { get; set; } = string.Empty; 

        [Column("Cena_Ewidencyjna")]
        public decimal CenaEwidencyjna { get; set; }

        public decimal VAT { get; set; }

        [Required]
        [Column("Jednostka_Miary")]
        [MaxLength(20)]
        public string JednostkaMiary { get; set; } = string.Empty;

        [Column("czas_trwania_w_min")]
        public int CzasTrwaniaWMin { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Pozycja_Wizyty")]
    public class VisitItem
    {
        [Key]
        [Column("id_pozycji_wizyty")]
        public int Id { get; set; }

        [Required]
        [Column("id_wizyty")]
        public int MedicalVisitId { get; set; }

        [ForeignKey(nameof(MedicalVisitId))]
        public virtual MedicalVisit MedicalVisit { get; set; } = null!;

        [Required]
        [Column("id_katalogu")]
        public int CatalogItemId { get; set; }

        [ForeignKey(nameof(CatalogItemId))]
        public virtual CatalogItem CatalogItem { get; set; } = null!;

        [Required]
        [Column("Ilosc", TypeName = "decimal(8,2)")]
        public decimal Quantity { get; set; } = 1;

        [Required]
        [Column("Cena_Zastosowana", TypeName = "decimal(8,2)")]
        public decimal AppliedPrice { get; set; }

        [Required]
        [Column("VAT_Zastosowany", TypeName = "decimal(5,2)")]
        public decimal AppliedVAT { get; set; }
    }
}
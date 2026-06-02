using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Zwierzę")]
    public class Patient
    {
        [Key]
        [Column("id_zwierzęcia")]
        public int Id { get; set; }

        [Required]
        [Column("id_właściciela")]
        public int ClientId { get; set; }

        [Required]
        [Column("Imię")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column("Gatunek")]
        [MaxLength(50)]
        public string Species { get; set; } = string.Empty;

        [Column("Rasa")]
        [MaxLength(50)]
        public string? Breed { get; set; }

        [Required]
        [Column("Płeć")]
        [MaxLength(1)]
        public string Gender { get; set; } = string.Empty;

        [Column("Data_Urodzenia")]
        public DateTime? DateOfBirth { get; set; }

        [Column("waga")]
        public decimal? Weight { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

    }

}
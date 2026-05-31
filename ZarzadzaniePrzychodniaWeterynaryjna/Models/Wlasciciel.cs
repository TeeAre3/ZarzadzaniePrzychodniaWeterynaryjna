using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Właściciel")]
    public class Wlasciciel
    {
        [Key] 
        [Column("id_właściciela")]
        public int Id { get; set; }

        [Column("Telefon")]
        [MaxLength(15)] 
        public string? Telefon { get; set; }

        [Column("Imię")]
        [MaxLength(50)]
        public string? Imie { get; set; }

        [Column("Nazwisko")]
        [MaxLength(50)]
        public string? Nazwisko { get; set; }

        [Column("Nazwa_firmy")]
        [MaxLength(100)]
        public string? NazwaFirmy { get; set; }

        [Required] 
        [Column("Email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("Data_rejestracji")]
        public DateTime DataRejestracji { get; set; } = DateTime.Now;

        [NotMapped]
        public string WyswietlanaNazwa
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NazwaFirmy) && !string.IsNullOrWhiteSpace(Nazwisko))
                    return $"{NazwaFirmy} ({Imie} {Nazwisko})";

                if (!string.IsNullOrWhiteSpace(NazwaFirmy))
                    return NazwaFirmy;

                return $"{Imie} {Nazwisko}".Trim();
            }
        }

        public ICollection<Zwierze> Zwierzeta { get; set; } = new List<Zwierze>();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Właściciel")]
    public class Client
    {
        [Key] 
        [Column("id_właściciela")]
        public int Id { get; set; }

        [Column("Telefon")]
        [MaxLength(15)] 
        public string? PhoneNumber { get; set; }

        [Column("Imię")]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Column("Nazwisko")]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Column("Nazwa_firmy")]
        [MaxLength(100)]
        public string? CompanyName { get; set; }

        [Required] 
        [Column("Email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("Data_rejestracji")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CompanyName) && !string.IsNullOrWhiteSpace(LastName))
                    return $"{CompanyName} ({FirstName} {LastName})";

                if (!string.IsNullOrWhiteSpace(CompanyName))
                    return CompanyName;

                return $"{FirstName} {LastName}".Trim();
            }
        }

        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Wizyta_Medyczna")]
    public class MedicalVisit
    {
        [Key]
        [Column("id_wizyty")]
        public int Id { get; set; }

        [Column("id_rezerwacji")]
        public int AppointmentId { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment? Appointment { get; set; }

        [Required]
        [Column("Data_Wizyty")]
        public DateTime VisitDate { get; set; }

        [Column("Opis_Wywiadu")]
        public string? Interview { get; set; }

        [Column("Rozpoznanie")]
        public string? Diagnosis { get; set; }

        [Column("Zalecenia")]
        public string? Recommendations { get; set; }

        public virtual ICollection<VisitItem> VisitItems { get; set; } = [];
    }
}
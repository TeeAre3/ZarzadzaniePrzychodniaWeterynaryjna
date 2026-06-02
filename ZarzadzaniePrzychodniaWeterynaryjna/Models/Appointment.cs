using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Models
{
    [Table("Harmonogram")]
    public class Appointment
    {
        [Key]
        [Column("id_rezerwacji")]
        public int Id { get; set; }

        [Column("id_zwierzęcia")]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public virtual Patient? Patient { get; set; }

        [Required]
        [Column("Planowana_Data_Rozpoczęcia")]
        public DateTime ScheduledDate { get; set; }

        [Column("Szacowany_czas_trwania")]
        public int EstimatedDurationMin { get; set; }

        [Column("Powód_Wizyty")]
        [MaxLength(150)]
        public string? Reason { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Status_Wizyty")]
        public string Status { get; set; } = "Planowana"; 

        [Column("Rzeczywisty_Czas_Rozpoczęcia")]
        public DateTime? ActualStartTime { get; set; }

        [Column("Rzeczywisty_Czas_Zakończenia")]
        public DateTime? ActualEndTime { get; set; }
    }
}
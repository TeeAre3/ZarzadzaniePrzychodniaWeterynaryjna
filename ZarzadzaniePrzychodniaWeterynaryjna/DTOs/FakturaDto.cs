using System;

namespace ZarzadzaniePrzychodniaWeterynaryjna.DTOs
{
    public class FakturaDto
    {
        public int WizytaId { get; set; }
        public DateTime Data { get; set; }
        public string Klient { get; set; } = string.Empty;
        public string Pacjent { get; set; } = string.Empty;
        public decimal Suma { get; set; }
    }
}
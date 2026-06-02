using System;

namespace ZarzadzaniePrzychodniaWeterynaryjna.DTOs
{
    public class InvoiceDto
    {
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string Client { get; set; } = string.Empty;
        public string Patient { get; set; } = string.Empty;
        public decimal TotalAmmount { get; set; }
    }
}
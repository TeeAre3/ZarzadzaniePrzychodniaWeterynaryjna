using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.DTOs;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class StatisticsRepository(ApplicationDbContext context)
    {
        public List<InvoiceDto> GetInvoices()
        {
            return [.. context.MedicalVisits
                .Include(v => v.Appointment!).ThenInclude(a => a.Patient!).ThenInclude(p => p.Client!)
                .Include(v => v.VisitItems)
                .OrderByDescending(v => v.VisitDate)
                .Select(v => new InvoiceDto
                {
                    AppointmentId = v.Id,
                    Date = v.VisitDate,
                    Client = v.Appointment!.Patient!.Client!.DisplayName ?? "Brak danych",
                    Patient = v.Appointment!.Patient!.Name ?? "Brak danych",
                    TotalAmmount = v.VisitItems.Sum(p => p.AppliedPrice * p.Quantity * (1 + (p.AppliedVAT / 100m)))
                })];
        }
    }
}
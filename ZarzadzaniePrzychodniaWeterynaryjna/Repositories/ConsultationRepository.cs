using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class ConsultationRepository(ApplicationDbContext context)
    {
        public List<MedicalVisit> GetPatientHistory(int patientId)
        {
            return context.MedicalVisits
                .Where(v => v.Appointment != null && v.Appointment.PatientId == patientId)
                .OrderByDescending(v => v.VisitDate)
                .ToList();
        }

        public void SaveConsultation(MedicalVisit visit, IEnumerable<VisitItem> items)
        {
            context.MedicalVisits.Add(visit);
            context.SaveChanges();

            foreach (var item in items)
            {
                var newItem = new VisitItem
                {
                    MedicalVisitId = visit.Id,
                    CatalogItemId = item.CatalogItemId,
                    Quantity = item.Quantity,
                    AppliedPrice = item.AppliedPrice,
                    AppliedVAT = item.AppliedVAT
                };
                context.VisitItems.Add(newItem);
            }

            var existingAppointment = context.Appointments.Find(visit.AppointmentId);
            if (existingAppointment != null)
            {
                existingAppointment.Status = "Zrealizowana";
            }

            context.SaveChanges();
        }
    }
}
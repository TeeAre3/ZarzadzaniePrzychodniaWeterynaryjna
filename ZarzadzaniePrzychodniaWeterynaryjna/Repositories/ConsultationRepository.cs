using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class ConsultationRepository
    {
        public List<MedicalVisit> GetPatientHistory(int patientId)
        {
            using var db = new ApplicationDbContext();
            return db.MedicalVisits
                .Where(v => v.Appointment != null && v.Appointment.PatientId == patientId)
                .OrderByDescending(v => v.VisitDate)
                .ToList();
        }

        public void SaveConsultation(MedicalVisit visit, IEnumerable<VisitItem> items)
        {
            using var db = new ApplicationDbContext();

            db.MedicalVisits.Add(visit);
            db.SaveChanges();

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
                db.VisitItems.Add(newItem);
            }

            var existingAppointment = db.Appointments.Find(visit.AppointmentId);
            if (existingAppointment != null)
            {
                existingAppointment.Status = "Zrealizowana";
            }

            db.SaveChanges();
        }
    }
}
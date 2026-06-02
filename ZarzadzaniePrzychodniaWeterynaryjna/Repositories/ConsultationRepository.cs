using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class ConsultationRepository
    {
        private readonly ApplicationDbContext _context;

        public ConsultationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MedicalVisit> GetPatientHistory(int patientId)
        {
            return _context.MedicalVisits
                .Where(v => v.Appointment != null && v.Appointment.PatientId == patientId)
                .OrderByDescending(v => v.VisitDate)
                .ToList();
        }

        public void SaveConsultation(MedicalVisit visit, IEnumerable<VisitItem> items)
        {
            _context.MedicalVisits.Add(visit);
            _context.SaveChanges();

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
                _context.VisitItems.Add(newItem);
            }

            var existingAppointment = _context.Appointments.Find(visit.AppointmentId);
            if (existingAppointment != null)
            {
                existingAppointment.Status = "Zrealizowana";
            }

            _context.SaveChanges();
        }
    }
}
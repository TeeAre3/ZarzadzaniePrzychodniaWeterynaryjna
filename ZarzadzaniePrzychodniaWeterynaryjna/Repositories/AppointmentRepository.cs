using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class AppointmentRepository
    {
        public List<Appointment> GetPlannedAppointments()
        {
            using var db = new ApplicationDbContext();
            return db.Appointments
                .Include(a => a.Patient)
                .Where(a => a.Status == "Planowana")
                .OrderBy(a => a.ScheduledDate)
                .ToList();
        }

        public void AddAppointment(Appointment appointment)
        {
            using var db = new ApplicationDbContext();
            db.Appointments.Add(appointment);
            db.SaveChanges();
        }

        public void RemoveAppointment(Appointment appointment)
        {
            using var db = new ApplicationDbContext();
            db.Appointments.Remove(appointment);
            db.SaveChanges();
        }
    }
}
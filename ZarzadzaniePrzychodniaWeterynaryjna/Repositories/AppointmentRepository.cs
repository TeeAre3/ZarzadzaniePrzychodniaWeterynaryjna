using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class AppointmentRepository(ApplicationDbContext context)
    {
        public List<Appointment> GetPlannedAppointments()
        {
            return context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.Status == "Planowana")
                .OrderBy(a => a.ScheduledDate)
                .ToList();
        }

        public void AddAppointment(Appointment appointment)
        {
            context.Appointments.Add(appointment);
            context.SaveChanges();
        }

        public void RemoveAppointment(Appointment appointment)
        {
            context.Appointments.Remove(appointment);
            context.SaveChanges();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class AppointmentRepository(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        public List<Appointment> GetPlannedAppointments()
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.Status == "Planowana")
                .OrderBy(a => a.ScheduledDate)
                .ToList();
        }

        public void AddAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        public void RemoveAppointment(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();
        }
    }
}
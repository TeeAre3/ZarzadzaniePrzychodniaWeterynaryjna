using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;
using System.Runtime.CompilerServices;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class PatientRepository(ApplicationDbContext context)
    {
        public async Task<List<Patient>> GetPatientsAsync(int clientId)
        {
            return await context.Patients.Where(p => p.ClientId == clientId).ToListAsync();
        }

        public void AddPatient(Patient patient)
        {
            context.Patients.Add(patient);
            context.SaveChanges();
        }
        public void UpdatePatient(Patient patient)
        {
            context.Patients.Update(patient);
            context.SaveChanges();
        }
        public void RemovePatient(Patient patient)
        {
            context.Patients.Remove(patient);
            context.SaveChanges();
        }
         public void SaveAllChanges()
        {
            context.SaveChanges();
        }

        public bool HasChanges()
        {
            context.ChangeTracker.DetectChanges();
            return context.ChangeTracker.HasChanges();
        }
    }
}

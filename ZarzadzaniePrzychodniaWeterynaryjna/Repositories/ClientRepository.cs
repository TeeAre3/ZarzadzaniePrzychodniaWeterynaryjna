using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class ClientRepository
    {
        public async Task<List<Client>> GetClientsAsync()
        {
            using var db = new ApplicationDbContext();
            return await db.Clients.ToListAsync();
        }

        public async Task<List<Patient>> GetPatientsAsync(int clientId)
        {
            using var db = new ApplicationDbContext();
            return await db.Patients.Where(p => p.ClientId == clientId).ToListAsync();
        }

        public void AddClient(Client client)
        {
            using var db = new ApplicationDbContext();
            db.Clients.Add(client);
            db.SaveChanges();
        }

        public void AddPatient(Patient patient)
        {
            using var db = new ApplicationDbContext();
            db.Patients.Add(patient);
            db.SaveChanges();
        }
    }
}
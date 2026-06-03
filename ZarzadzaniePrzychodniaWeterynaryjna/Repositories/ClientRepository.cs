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
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<List<Patient>> GetPatientsAsync(int clientId)
        {
            return await _context.Patients.Where(p => p.ClientId == clientId).ToListAsync();
        }

        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        public void AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }
        public void UpdateClient(Client client)
        {
            _context.Clients.Update(client);
            _context.SaveChanges();
        }

        public void UpdatePatient(Patient patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }
        public void RemoveClient(Client client)
        {
            _context.Clients.Remove(client);
            _context.SaveChanges();
        }

        public void RemovePatient(Patient patient)
        {
            _context.Patients.Remove(patient);
            _context.SaveChanges();
        }

        public void SaveAllChanges()
        {
            _context.SaveChanges();
        }

        public bool HasChanges()
        {
            _context.ChangeTracker.DetectChanges();
            return _context.ChangeTracker.HasChanges();
        }
    }
}
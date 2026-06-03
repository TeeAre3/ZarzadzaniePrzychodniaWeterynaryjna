using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class ClientRepository(ApplicationDbContext context)
    {
        public async Task<List<Client>> GetClientsAsync()
        {
            return await context.Clients.ToListAsync();
        }

        public void AddClient(Client client)
        {
            context.Clients.Add(client);
            context.SaveChanges();
        }

        public void UpdateClient(Client client)
        {
            context.Clients.Update(client);
            context.SaveChanges();
        }

        public void RemoveClient(Client client)
        {
            context.Clients.Remove(client);
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
using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class CatalogRepository(ApplicationDbContext context)
    {
        private readonly ApplicationDbContext _context = context;

        public List<CatalogItem> GetCatalog()
        {
            return [.. _context.CatalogItems.OrderBy(c => c.Name)];
        }

        public void AddItem(CatalogItem item)
        {
            _context.CatalogItems.Add(item);
            _context.SaveChanges();
        }
    }
}
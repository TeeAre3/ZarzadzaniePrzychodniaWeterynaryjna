using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class CatalogRepository
    {
        public List<CatalogItem> GetCatalog()
        {
            using var db = new ApplicationDbContext();
            return db.CatalogItems.OrderBy(c => c.Name).ToList();
        }

        public void AddItem(CatalogItem item)
        {
            using var db = new ApplicationDbContext();
            db.CatalogItems.Add(item);
            db.SaveChanges();
        }
    }
}
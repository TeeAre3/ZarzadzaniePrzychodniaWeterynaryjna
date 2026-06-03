using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Repositories
{
    public class CatalogRepository(ApplicationDbContext context)
    {
        public List<CatalogItem> GetCatalog()
        {
            return [.. context.CatalogItems.OrderBy(c => c.Name)];
        }

        public void AddItem(CatalogItem item)
        {
            context.CatalogItems.Add(item);
            context.SaveChanges();
        }

        public void RemoveItem(CatalogItem item)
        {
            bool isUsed = context.VisitItems.Any(v => v.CatalogItemId == item.Id);

            if (isUsed)
            {
                throw new System.InvalidOperationException("Nie można usunąć tej pozycji. Została już użyta w historycznej wizycie pacjenta.");
            }

            context.CatalogItems.Remove(item);
            context.SaveChanges();
        }
    }
}
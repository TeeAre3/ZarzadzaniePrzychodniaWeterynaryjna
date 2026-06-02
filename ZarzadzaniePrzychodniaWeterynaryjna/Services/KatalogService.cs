using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Services
{
    public class KatalogService
    {
        public List<Katalog> PobierzKatalog()
        {
            using var db = new ApplicationDbContext();
            return db.Katalogi.OrderBy(k => k.Nazwa).ToList();
        }

        public void DodajPozycje(Katalog pozycja)
        {
            using var db = new ApplicationDbContext();
            db.Katalogi.Add(pozycja);
            db.SaveChanges();
        }
    }
}
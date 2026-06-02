using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Services
{
    public class HarmonogramService
    {
        public List<Harmonogram> PobierzPlanowaneWizyty()
        {
            using var db = new ApplicationDbContext();
            return db.Harmonogramy
                .Include(h => h.Zwierze)
                .Where(h => h.StatusWizyty == "Planowana")
                .OrderBy(h => h.PlanowanaDataRozpoczecia)
                .ToList();
        }

        public void DodajRezerwacje(Harmonogram rezerwacja)
        {
            using var db = new ApplicationDbContext();
            db.Harmonogramy.Add(rezerwacja);
            db.SaveChanges();
        }

        public void UsunRezerwacje(Harmonogram rezerwacja)
        {
            using var db = new ApplicationDbContext();
            db.Harmonogramy.Remove(rezerwacja);
            db.SaveChanges();
        }
    }
}
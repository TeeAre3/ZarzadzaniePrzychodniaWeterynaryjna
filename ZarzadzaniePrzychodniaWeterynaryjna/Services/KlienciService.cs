using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Services
{
    public class KlienciService
    {
        public async Task<List<Wlasciciel>> PobierzWlascicieliAsync()
        {
            using var db = new ApplicationDbContext();
            return await db.Wlasciciele.ToListAsync();
        }

        public async Task<List<Zwierze>> PobierzZwierzetaAsync(int wlascicielId)
        {
            using var db = new ApplicationDbContext();
            return await db.Zwierzeta.Where(z => z.WlascicielId == wlascicielId).ToListAsync();
        }

        public void DodajWlasciciela(Wlasciciel wlasciciel)
        {
            using var db = new ApplicationDbContext();
            db.Wlasciciele.Add(wlasciciel);
            db.SaveChanges();
        }

        public void DodajZwierze(Zwierze zwierze)
        {
            using var db = new ApplicationDbContext();
            db.Zwierzeta.Add(zwierze);
            db.SaveChanges();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.DTOs;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Services
{
    public class StatystykiService
    {
        public List<FakturaDto> PobierzFaktury()
        {
            using var db = new ApplicationDbContext();

            var wizyty = db.Wizyty
                .Include(w => w.Rezerwacja!).ThenInclude(r => r.Zwierze!).ThenInclude(z => z.Wlasciciel!)
                .Include(w => w.WykorzystaneUslugiILeki)
                .OrderByDescending(w => w.DataWizyty)
                .ToList();

            var faktury = wizyty.Select(w => new FakturaDto
            {
                WizytaId = w.Id,
                Data = w.DataWizyty,
                Klient = w.Rezerwacja?.Zwierze?.Wlasciciel?.WyswietlanaNazwa ?? "Brak danych",
                Pacjent = w.Rezerwacja?.Zwierze?.Imie ?? "Brak danych",
                Suma = w.WykorzystaneUslugiILeki.Sum(p => p.CenaZastosowana * p.Ilosc * (1 + (p.VATZastosowany / 100m)))
            }).ToList();

            return faktury;
        }
    }
}
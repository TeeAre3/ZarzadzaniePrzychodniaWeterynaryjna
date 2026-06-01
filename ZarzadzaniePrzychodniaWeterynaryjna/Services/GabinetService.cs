using System.Collections.Generic;
using System.Linq;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna.Services
{
    public class GabinetService
    {
        public List<WizytaMedyczna> PobierzHistorieZwierzecia(int zwierzeId)
        {
            using var db = new ApplicationDbContext();
            return db.Wizyty
                .Where(w => w.Rezerwacja != null && w.Rezerwacja.ZwierzeId == zwierzeId)
                .OrderByDescending(w => w.DataWizyty)
                .ToList();
        }

        public void ZapiszWizyte(WizytaMedyczna wizyta, IEnumerable<PozycjaWizyty> pozycje)
        {
            using var db = new ApplicationDbContext();

            db.Wizyty.Add(wizyta);
            db.SaveChanges();

            foreach (var poz in pozycje)
            {
                var nowaPozycja = new PozycjaWizyty
                {
                    WizytaId = wizyta.Id,
                    KatalogId = poz.KatalogId,
                    Ilosc = poz.Ilosc,
                    CenaZastosowana = poz.CenaZastosowana,
                    VATZastosowany = poz.VATZastosowany
                };
                db.PozycjeWizyt.Add(nowaPozycja);
            }

            var rezerwacjaWBazie = db.Harmonogramy.Find(wizyta.RezerwacjaId);
            if (rezerwacjaWBazie != null)
            {
                rezerwacjaWBazie.StatusWizyty = "Zrealizowana";
            }

            db.SaveChanges();
        }
    }
}
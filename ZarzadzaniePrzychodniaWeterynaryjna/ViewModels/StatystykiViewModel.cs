using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public class FakturaDto
    {
        public int WizytaId { get; set; }
        public DateTime Data { get; set; }
        public string Klient { get; set; } = string.Empty;
        public string Pacjent { get; set; } = string.Empty;
        public decimal Suma { get; set; }
    }

    public partial class StatystykiViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<FakturaDto> _listaFaktur = new();
        [ObservableProperty] private decimal _calkowityPrzychod = 0;

        public StatystykiViewModel()
        {
            ZaladujStatystyki();

            WeakReferenceMessenger.Default.Register<WizytaZakonczonaMessage>(this, (r, m) => ZaladujStatystyki());
        }

        private void ZaladujStatystyki()
        {
            using var db = new ApplicationDbContext();

            var wizyty = db.Wizyty
                .Include(w => w.Rezerwacja).ThenInclude(r => r.Zwierze).ThenInclude(z => z.Wlasciciel)
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

            ListaFaktur = new ObservableCollection<FakturaDto>(faktury);
            CalkowityPrzychod = faktury.Sum(f => f.Suma);
        }
    }
}
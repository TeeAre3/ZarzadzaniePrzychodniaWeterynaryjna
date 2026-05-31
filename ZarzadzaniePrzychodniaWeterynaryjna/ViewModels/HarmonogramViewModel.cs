using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class HarmonogramViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Harmonogram> _harmonogramLista = new();
        [ObservableProperty] private ObservableCollection<Wlasciciel> _wlascicieleLista = new();
        [ObservableProperty] private ObservableCollection<Zwierze> _zwierzetaLista = new();

        [ObservableProperty] private ObservableCollection<Katalog> _uslugiLista = new();

        [ObservableProperty] private Wlasciciel? _wybranyWlasciciel;
        partial void OnWybranyWlascicielChanged(Wlasciciel? value) => ZaladujZwierzeta();

        [ObservableProperty] private Zwierze? _wybraneZwierze;

        [ObservableProperty] private Katalog? _wybranaUsluga;
        partial void OnWybranaUslugaChanged(Katalog? value)
        {
            WyliczSzacowanyCzas();
        }

        [ObservableProperty] private DateTime _planowanaData = DateTime.Today;
        [ObservableProperty] private string _planowanaGodzina = "10:00";

        [ObservableProperty] private int _szacowanyCzasMin = 15;

        public HarmonogramViewModel()
        {
            ZaladujWlascicieli();
            ZaladujUslugi(); 
            ZaladujHarmonogram();

            WeakReferenceMessenger.Default.Register<KatalogZmienionyMessage>(this, (r, m) =>
            {
                ZaladujUslugi();
            });
        }

        private void ZaladujWlascicieli()
        {
            using var db = new ApplicationDbContext();
            WlascicieleLista = new ObservableCollection<Wlasciciel>(db.Wlasciciele.ToList());
        }

        private void ZaladujZwierzeta()
        {
            if (WybranyWlasciciel == null)
            {
                ZwierzetaLista.Clear();
                return;
            }
            using var db = new ApplicationDbContext();
            ZwierzetaLista = new ObservableCollection<Zwierze>(db.Zwierzeta.Where(z => z.WlascicielId == WybranyWlasciciel.Id).ToList());
        }

        private void ZaladujUslugi()
        {
            using var db = new ApplicationDbContext();
            var pobraneUslugi = db.Katalogi.Where(k => k.Typ == "Usługa").ToList();
            UslugiLista = new ObservableCollection<Katalog>(pobraneUslugi);
        }

        private void ZaladujHarmonogram()
        {
            using var db = new ApplicationDbContext();
            var pobrane = db.Harmonogramy
                            .Include(h => h.Zwierze)
                            .OrderByDescending(h => h.PlanowanaDataRozpoczecia)
                            .ToList();
            HarmonogramLista = new ObservableCollection<Harmonogram>(pobrane);
        }

        private void WyliczSzacowanyCzas()
        {
            if (WybranaUsluga == null) return;

            int czasBazowy = WybranaUsluga.CzasTrwaniaWMin;

            int czasModyfikowany = czasBazowy + 0;

            int wielkoscBloku = 15;
            int zaokraglonyCzas;

            if (czasModyfikowany % wielkoscBloku == 0)
            {
                zaokraglonyCzas = czasModyfikowany;
            }
            else
            {
                zaokraglonyCzas = ((czasModyfikowany / wielkoscBloku) + 1) * wielkoscBloku;
            }

            SzacowanyCzasMin = zaokraglonyCzas;
        }

        [RelayCommand]
        private void DodajRezerwacje()
        {
            if (WybraneZwierze == null)
            {
                MessageBox.Show("Wybierz pacjenta!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(PlanowanaGodzina, out TimeSpan parsedTime))
            {
                MessageBox.Show("Wprowadź godzinę w formacie (np. 14:30)!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nowaWizyta = new Harmonogram
            {
                ZwierzeId = WybraneZwierze.Id,
                PlanowanaDataRozpoczecia = PlanowanaData.Date.Add(parsedTime),
                SzacowanyCzasTrwania = SzacowanyCzasMin,
                StatusWizyty = "Planowana"
            };

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    db.Harmonogramy.Add(nowaWizyta);
                    db.SaveChanges();
                    MessageBox.Show("Zarezerwowano termin!", "Sukces");
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Błąd SQL: {ex.InnerException?.Message ?? ex.Message}", "Błąd rezerwacji", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            ZaladujHarmonogram();
        }

        [ObservableProperty] private Harmonogram? _wybranaRezerwacja;

        [RelayCommand]
        private void UsunRezerwacje()
        {
            if (WybranaRezerwacja == null)
            {
                MessageBox.Show("Wybierz wizytę do usunięcia z tabeli!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Czy na pewno usunąć rezerwację?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new ApplicationDbContext())
                {
                    try { db.Harmonogramy.Remove(WybranaRezerwacja); db.SaveChanges(); }
                    catch (System.Exception ex) { MessageBox.Show($"Błąd: {ex.Message}", "Błąd"); return; }
                }
                ZaladujHarmonogram();
            }
        }
    }
}
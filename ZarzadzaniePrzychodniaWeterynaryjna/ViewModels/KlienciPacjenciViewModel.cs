using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class KlienciPacjenciViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Wlasciciel> _wlascicieleLista = new();
        [ObservableProperty] private int _wybranyTypKlienta = 0;
        [ObservableProperty] private string _noweImie = string.Empty;
        [ObservableProperty] private string _noweNazwisko = string.Empty;
        [ObservableProperty] private string _nowaNazwaFirmy = string.Empty;
        [ObservableProperty] private string _nowyTelefon = string.Empty;
        [ObservableProperty] private string _nowyEmail = string.Empty;

        [ObservableProperty] private ObservableCollection<Zwierze> _zwierzetaLista = new();
        [ObservableProperty] private Wlasciciel? _wybranyWlasciciel;

        partial void OnWybranyWlascicielChanged(Wlasciciel? value) => ZaladujZwierzeta();

        [ObservableProperty] private string _noweZwierzeImie = string.Empty;
        [ObservableProperty] private string _noweZwierzeGatunek = string.Empty;
        [ObservableProperty] private string _noweZwierzeRasa = string.Empty;
        [ObservableProperty] private int _noweZwierzePlecIndex = 0;
        [ObservableProperty] private decimal? _noweZwierzeWaga;

        public KlienciPacjenciViewModel()
        {
            ZaladujWlascicieli();
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

        [RelayCommand]
        private void DodajWlasciciela()
        {
            if (string.IsNullOrWhiteSpace(NowyEmail))
            {
                MessageBox.Show("Email jest wymagany!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((WybranyTypKlienta == 0 || WybranyTypKlienta == 2) && (string.IsNullOrWhiteSpace(NoweImie) || string.IsNullOrWhiteSpace(NoweNazwisko)))
            {
                MessageBox.Show("Uzupełnij Imię i Nazwisko!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((WybranyTypKlienta == 1 || WybranyTypKlienta == 2) && string.IsNullOrWhiteSpace(NowaNazwaFirmy))
            {
                MessageBox.Show("Uzupełnij Nazwę Firmy!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nowyWlasciciel = new Wlasciciel
            {
                Imie = string.IsNullOrWhiteSpace(NoweImie) ? null : NoweImie,
                Nazwisko = string.IsNullOrWhiteSpace(NoweNazwisko) ? null : NoweNazwisko,
                NazwaFirmy = string.IsNullOrWhiteSpace(NowaNazwaFirmy) ? null : NowaNazwaFirmy,
                Telefon = string.IsNullOrWhiteSpace(NowyTelefon) ? null : NowyTelefon,
                Email = NowyEmail,
                DataRejestracji = System.DateTime.Now
            };

            using (var db = new ApplicationDbContext())
            {
                try 
                { 
                    db.Wlasciciele.Add(nowyWlasciciel); db.SaveChanges(); MessageBox.Show("Dodano klienta!", "Sukces");
                    WeakReferenceMessenger.Default.Send(new WlascicielZmienionyMessage());
                }
                catch (System.Exception ex) { MessageBox.Show($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }
            }

            ZaladujWlascicieli();
            NoweImie = NoweNazwisko = NowaNazwaFirmy = NowyTelefon = NowyEmail = string.Empty;
        }

        [RelayCommand]
        private void DodajZwierze()
        {
            if (WybranyWlasciciel == null) { MessageBox.Show("Wybierz właściciela!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            if (string.IsNullOrWhiteSpace(NoweZwierzeImie) || string.IsNullOrWhiteSpace(NoweZwierzeGatunek)) { MessageBox.Show("Uzupełnij Imię i Gatunek!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (NoweZwierzeWaga <= 0) { MessageBox.Show("Waga musi być > 0!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var noweZwierze = new Zwierze
            {
                WlascicielId = WybranyWlasciciel.Id,
                Imie = NoweZwierzeImie,
                Gatunek = NoweZwierzeGatunek,
                Rasa = string.IsNullOrWhiteSpace(NoweZwierzeRasa) ? null : NoweZwierzeRasa,
                Plec = NoweZwierzePlecIndex == 0 ? "S" : "M",
                Waga = NoweZwierzeWaga
            };

            using (var db = new ApplicationDbContext())
            {
                try 
                { 
                    db.Zwierzeta.Add(noweZwierze); db.SaveChanges(); MessageBox.Show("Dodano pacjenta!", "Sukces");
                    WeakReferenceMessenger.Default.Send(new ZwierzeZmienioneMessage());
                }
                catch (System.Exception ex) { MessageBox.Show($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }
            }

            ZaladujZwierzeta();
            NoweZwierzeImie = NoweZwierzeGatunek = NoweZwierzeRasa = string.Empty; NoweZwierzeWaga = null;
        }
    }
}
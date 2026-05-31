using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class KatalogViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Katalog> _katalogLista = new();
        [ObservableProperty] private string _nowyKatalogNazwa = string.Empty;
        [ObservableProperty] private int _nowyKatalogTypIndex = 0;
        [ObservableProperty] private decimal? _nowyKatalogCena;
        [ObservableProperty] private decimal? _nowyKatalogVAT = 23;
        [ObservableProperty] private string _nowyKatalogJednostka = string.Empty;
        [ObservableProperty] private int _nowyKatalogCzasMin = 15;

        public KatalogViewModel()
        {
            ZaladujKatalog();
        }

        private void ZaladujKatalog()
        {
            using var db = new ApplicationDbContext();
            KatalogLista = new ObservableCollection<Katalog>(db.Katalogi.ToList());
        }

        [RelayCommand]
        private void DodajKatalog()
        {
            if (string.IsNullOrWhiteSpace(NowyKatalogNazwa) || string.IsNullOrWhiteSpace(NowyKatalogJednostka)) { MessageBox.Show("Uzupełnij Nazwę i Jednostkę!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if ((NowyKatalogCena ?? 0) < 0 || (NowyKatalogVAT ?? 0) < 0) { MessageBox.Show("Cena/VAT ujemne!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var nowaPozycja = new Katalog
            {
                Nazwa = NowyKatalogNazwa,
                Typ = NowyKatalogTypIndex switch { 0 => "Usługa", 1 => "Towar", 2 => "Lek", _ => "Inne" },
                CenaEwidencyjna = NowyKatalogCena ?? 0,
                VAT = NowyKatalogVAT ?? 0,
                JednostkaMiary = NowyKatalogJednostka,
                CzasTrwaniaWMin = NowyKatalogCzasMin
            };

            using (var db = new ApplicationDbContext())
            {
                try 
                { 
                    db.Katalogi.Add(nowaPozycja);
                    db.SaveChanges(); 
                    WeakReferenceMessenger.Default.Send(new KatalogZmienionyMessage()); 
                    MessageBox.Show("Dodano pozycję!", "Sukces"); 
                }
                catch (System.Exception ex) { MessageBox.Show($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }
            }

            ZaladujKatalog();
            NowyKatalogNazwa = NowyKatalogJednostka = string.Empty; NowyKatalogCena = null; NowyKatalogCzasMin = 15;
        }
    }
}
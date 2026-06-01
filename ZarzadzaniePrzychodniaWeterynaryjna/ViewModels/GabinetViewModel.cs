using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Services;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class GabinetViewModel : ObservableObject
    {
        private readonly GabinetService _gabinetService = new();
        private readonly KatalogService _katalogService = new();

        [ObservableProperty] private Harmonogram? _aktywnaRezerwacja;
        [ObservableProperty] private ObservableCollection<WizytaMedyczna> _historiaWizyt = new();

        [ObservableProperty] private string _nowyWywiad = string.Empty;
        [ObservableProperty] private string _noweRozpoznanie = string.Empty;
        [ObservableProperty] private string _noweZalecenia = string.Empty;

        [ObservableProperty] private ObservableCollection<Katalog> _katalogDostepny = new();
        [ObservableProperty] private ObservableCollection<PozycjaWizyty> _dodanePozycje = new();
        [ObservableProperty] private Katalog? _wybranyElementKatalogu;
        [ObservableProperty] private decimal _nowaIlosc = 1;
        [ObservableProperty] private decimal _sumaCalkowita = 0;

        public GabinetViewModel()
        {
            WeakReferenceMessenger.Default.Register<KatalogZmienionyMessage>(this, (r, m) => ZaladujKatalog());
        }

        public void RozpocznijWizyte(Harmonogram rezerwacja)
        {
            AktywnaRezerwacja = rezerwacja;

            NowyWywiad = string.Empty;
            NoweRozpoznanie = string.Empty;
            NoweZalecenia = string.Empty;

            DodanePozycje.Clear();
            NowaIlosc = 1;
            SumaCalkowita = 0;

            ZaladujHistorie(rezerwacja.ZwierzeId);
            ZaladujKatalog();
        }

        private void ZaladujHistorie(int zwierzeId)
        {
            var historia = _gabinetService.PobierzHistorieZwierzecia(zwierzeId);
            HistoriaWizyt = new ObservableCollection<WizytaMedyczna>(historia);
        }

        private void ZaladujKatalog()
        {
            KatalogDostepny = new ObservableCollection<Katalog>(_katalogService.PobierzKatalog());
        }

        [RelayCommand]
        private void DodajPozycje()
        {
            if (WybranyElementKatalogu == null) return;
            if (NowaIlosc <= 0) { MessageBox.Show("Ilość musi być większa od 0.", "Błąd"); return; }

            var nowaPozycja = new PozycjaWizyty
            {
                KatalogId = WybranyElementKatalogu.Id,
                Katalog = WybranyElementKatalogu,
                Ilosc = NowaIlosc,
                CenaZastosowana = WybranyElementKatalogu.CenaEwidencyjna,
                VATZastosowany = WybranyElementKatalogu.VAT
            };

            DodanePozycje.Add(nowaPozycja);
            PrzeliczSume();

            NowaIlosc = 1;
            WybranyElementKatalogu = null;
        }

        [RelayCommand]
        private void UsunPozycje(PozycjaWizyty pozycja)
        {
            if (pozycja != null)
            {
                DodanePozycje.Remove(pozycja);
                PrzeliczSume();
            }
        }

        private void PrzeliczSume()
        {
            SumaCalkowita = DodanePozycje.Sum(p => p.CenaZastosowana * p.Ilosc * (1 + (p.VATZastosowany / 100m)));
        }

        [RelayCommand]
        private void ZakonczWizyte()
        {
            if (AktywnaRezerwacja == null) return;

            if (string.IsNullOrWhiteSpace(NowyWywiad) && string.IsNullOrWhiteSpace(NoweRozpoznanie))
            {
                MessageBox.Show("Uzupełnij Wywiad lub Rozpoznanie przed zakończeniem wizyty.", "Informacja");
                return;
            }

            var nowaWizyta = new WizytaMedyczna
            {
                RezerwacjaId = AktywnaRezerwacja.Id,
                DataWizyty = DateTime.Now,
                OpisWywiadu = string.IsNullOrWhiteSpace(NowyWywiad) ? null : NowyWywiad,
                Rozpoznanie = string.IsNullOrWhiteSpace(NoweRozpoznanie) ? null : NoweRozpoznanie,
                Zalecenia = string.IsNullOrWhiteSpace(NoweZalecenia) ? null : NoweZalecenia
            };

            try
            {
                _gabinetService.ZapiszWizyte(nowaWizyta, DodanePozycje);

                AktywnaRezerwacja = null;
                MessageBox.Show($"Wizyta zakończona. Suma do zapłaty: {SumaCalkowita:N2} zł", "Sukces");

                WeakReferenceMessenger.Default.Send(new WizytaZakonczonaMessage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd");
            }
        }
    }
}
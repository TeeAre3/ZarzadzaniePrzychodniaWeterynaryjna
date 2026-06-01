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
    public partial class HarmonogramViewModel : ObservableObject
    {
        private readonly HarmonogramService _harmonogramService = new();
        private readonly KlienciService _klienciService = new();
        private readonly KatalogService _katalogService = new();

        [ObservableProperty] private ObservableCollection<Harmonogram> _harmonogramLista = new();
        [ObservableProperty] private ObservableCollection<Wlasciciel> _wlascicieleLista = new();
        [ObservableProperty] private ObservableCollection<Zwierze> _zwierzetaLista = new();
        [ObservableProperty] private ObservableCollection<Katalog> _uslugiLista = new();

        [ObservableProperty] private Wlasciciel? _wybranyWlasciciel;
        partial void OnWybranyWlascicielChanged(Wlasciciel? value) => _ = ZaladujZwierzetaAsync();

        [ObservableProperty] private Zwierze? _wybraneZwierze;
        [ObservableProperty] private Katalog? _wybranaUsluga;
        partial void OnWybranaUslugaChanged(Katalog? value) => WyliczSzacowanyCzas();

        [ObservableProperty] private DateTime _planowanaData = DateTime.Today;
        [ObservableProperty] private string _planowanaGodzina = "10:00";
        [ObservableProperty] private int _szacowanyCzasMin = 15;
        [ObservableProperty] private Harmonogram? _wybranaRezerwacja;

        public HarmonogramViewModel()
        {
            _ = ZaladujWlascicieliAsync();
            ZaladujUslugi();
            ZaladujHarmonogram();

            WeakReferenceMessenger.Default.Register<KatalogZmienionyMessage>(this, (r, m) => ZaladujUslugi());
            WeakReferenceMessenger.Default.Register<WizytaZakonczonaMessage>(this, (r, m) => ZaladujHarmonogram());

            WeakReferenceMessenger.Default.Register<WlascicielZmienionyMessage>(this, (r, m) => _ = ZaladujWlascicieliAsync());
            WeakReferenceMessenger.Default.Register<ZwierzeZmienioneMessage>(this, (r, m) =>
            {
                if (WybranyWlasciciel != null) _ = ZaladujZwierzetaAsync();
            });
        }

        private async System.Threading.Tasks.Task ZaladujWlascicieliAsync()
        {
            var lista = await _klienciService.PobierzWlascicieliAsync();
            WlascicieleLista = new ObservableCollection<Wlasciciel>(lista);
        }

        private async System.Threading.Tasks.Task ZaladujZwierzetaAsync()
        {
            if (WybranyWlasciciel == null) { ZwierzetaLista.Clear(); return; }
            var lista = await _klienciService.PobierzZwierzetaAsync(WybranyWlasciciel.Id);
            ZwierzetaLista = new ObservableCollection<Zwierze>(lista);
        }

        private void ZaladujUslugi()
        {
            var wszystkiePozycje = _katalogService.PobierzKatalog();
            UslugiLista = new ObservableCollection<Katalog>(wszystkiePozycje.Where(k => k.Typ == "Usługa"));
        }

        private void ZaladujHarmonogram()
        {
            HarmonogramLista = new ObservableCollection<Harmonogram>(_harmonogramService.PobierzPlanowaneWizyty());
        }

        private void WyliczSzacowanyCzas()
        {
            if (WybranaUsluga == null) return;
            int czas = WybranaUsluga.CzasTrwaniaWMin;
            SzacowanyCzasMin = (czas % 15 == 0) ? czas : ((czas / 15) + 1) * 15;
        }

        [RelayCommand]
        private void DodajRezerwacje()
        {
            if (WybraneZwierze == null || !TimeSpan.TryParse(PlanowanaGodzina, out _)) { MessageBox.Show("Wybierz pacjenta i poprawną godzinę!"); return; }

            var nowaRezerwacja = new Harmonogram
            {
                ZwierzeId = WybraneZwierze.Id,
                PowodWizyty = WybranaUsluga?.Nazwa,
                PlanowanaDataRozpoczecia = PlanowanaData.Date.Add(TimeSpan.Parse(PlanowanaGodzina)),
                SzacowanyCzasTrwania = SzacowanyCzasMin,
                StatusWizyty = "Planowana"
            };

            try
            {
                _harmonogramService.DodajRezerwacje(nowaRezerwacja);
                ZaladujHarmonogram();
            }
            catch (Exception ex) { MessageBox.Show($"Błąd SQL: {ex.InnerException?.Message ?? ex.Message}"); }
        }

        [RelayCommand]
        private void UsunRezerwacje()
        {
            if (WybranaRezerwacja == null) return;

            try
            {
                _harmonogramService.UsunRezerwacje(WybranaRezerwacja);
                ZaladujHarmonogram();
            }
            catch (Exception ex) { MessageBox.Show($"Błąd: {ex.Message}"); }
        }

        [RelayCommand]
        private void RozpocznijWizyte()
        {
            if (WybranaRezerwacja == null) { MessageBox.Show("Wybierz rezerwację!"); return; }
            WeakReferenceMessenger.Default.Send(new PrzejdzDoGabinetuMessage(WybranaRezerwacja));
        }
    }
}
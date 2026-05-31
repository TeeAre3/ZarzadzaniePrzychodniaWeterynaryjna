using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
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
        partial void OnWybranaUslugaChanged(Katalog? value) => WyliczSzacowanyCzas();

        [ObservableProperty] private DateTime _planowanaData = DateTime.Today;
        [ObservableProperty] private string _planowanaGodzina = "10:00";
        [ObservableProperty] private int _szacowanyCzasMin = 15;
        [ObservableProperty] private Harmonogram? _wybranaRezerwacja;

        public HarmonogramViewModel()
        {
            ZaladujWlascicieli();
            ZaladujUslugi();
            ZaladujHarmonogram();

            WeakReferenceMessenger.Default.Register<KatalogZmienionyMessage>(this, (r, m) => ZaladujUslugi());
            WeakReferenceMessenger.Default.Register<WizytaZakonczonaMessage>(this, (r, m) => ZaladujHarmonogram());

            WeakReferenceMessenger.Default.Register<WlascicielZmienionyMessage>(this, (r, m) => ZaladujWlascicieli());
            WeakReferenceMessenger.Default.Register<ZwierzeZmienioneMessage>(this, (r, m) =>
            {
                if (WybranyWlasciciel != null) ZaladujZwierzeta();
            });
        }

        private void ZaladujWlascicieli() { using var db = new ApplicationDbContext(); WlascicieleLista = new ObservableCollection<Wlasciciel>(db.Wlasciciele.ToList()); }
        private void ZaladujZwierzeta() { if (WybranyWlasciciel == null) { ZwierzetaLista.Clear(); return; } using var db = new ApplicationDbContext(); ZwierzetaLista = new ObservableCollection<Zwierze>(db.Zwierzeta.Where(z => z.WlascicielId == WybranyWlasciciel.Id).ToList()); }
        private void ZaladujUslugi() { using var db = new ApplicationDbContext(); UslugiLista = new ObservableCollection<Katalog>(db.Katalogi.Where(k => k.Typ == "Usługa").ToList()); }
        private void ZaladujHarmonogram()
        {
            using var db = new ApplicationDbContext();
            HarmonogramLista = new ObservableCollection<Harmonogram>(db.Harmonogramy
                .Include(h => h.Zwierze)
                .Where(h => h.StatusWizyty == "Planowana")
                .OrderBy(h => h.PlanowanaDataRozpoczecia) 
                .ToList());
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

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    db.Harmonogramy.Add(new Harmonogram
                    {
                        ZwierzeId = WybraneZwierze.Id,
                        PowodWizyty = WybranaUsluga?.Nazwa,
                        PlanowanaDataRozpoczecia = PlanowanaData.Date.Add(TimeSpan.Parse(PlanowanaGodzina)),
                        SzacowanyCzasTrwania = SzacowanyCzasMin,
                        StatusWizyty = "Planowana"
                    });
                    db.SaveChanges();
                }
                catch (Exception ex) { MessageBox.Show($"Błąd SQL: {ex.InnerException?.Message ?? ex.Message}"); return; }
            }
            ZaladujHarmonogram();
        }

        [RelayCommand]
        private void UsunRezerwacje()
        {
            if (WybranaRezerwacja == null) return;
            using (var db = new ApplicationDbContext()) { db.Harmonogramy.Remove(WybranaRezerwacja); db.SaveChanges(); }
            ZaladujHarmonogram();
        }

        [RelayCommand]
        private void RozpocznijWizyte()
        {
            if (WybranaRezerwacja == null) { MessageBox.Show("Wybierz rezerwację!"); return; }
            WeakReferenceMessenger.Default.Send(new PrzejdzDoGabinetuMessage(WybranaRezerwacja));
        }


    }
}
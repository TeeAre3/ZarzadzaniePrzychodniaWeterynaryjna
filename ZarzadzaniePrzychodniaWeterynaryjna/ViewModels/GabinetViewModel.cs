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
    public partial class GabinetViewModel : ObservableObject
    {
        [ObservableProperty] private Harmonogram? _aktywnaRezerwacja;
        [ObservableProperty] private ObservableCollection<WizytaMedyczna> _historiaWizyt = new();

        [ObservableProperty] private string _nowyWywiad = string.Empty;
        [ObservableProperty] private string _noweRozpoznanie = string.Empty;
        [ObservableProperty] private string _noweZalecenia = string.Empty;

        public void RozpocznijWizyte(Harmonogram rezerwacja)
        {
            AktywnaRezerwacja = rezerwacja;

            NowyWywiad = string.Empty;
            NoweRozpoznanie = string.Empty;
            NoweZalecenia = string.Empty;

            ZaladujHistorie(rezerwacja.ZwierzeId);
        }

        private void ZaladujHistorie(int zwierzeId)
        {
            using var db = new ApplicationDbContext();
            var historia = db.Wizyty
                .Where(w => w.Rezerwacja != null && w.Rezerwacja.ZwierzeId == zwierzeId)
                .OrderByDescending(w => w.DataWizyty)
                .ToList();

            HistoriaWizyt = new ObservableCollection<WizytaMedyczna>(historia);
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

            using (var db = new ApplicationDbContext())
            {
                try
                {
                    var nowaWizyta = new WizytaMedyczna
                    {
                        RezerwacjaId = AktywnaRezerwacja.Id,
                        DataWizyty = DateTime.Now,
                        OpisWywiadu = string.IsNullOrWhiteSpace(NowyWywiad) ? null : NowyWywiad,
                        Rozpoznanie = string.IsNullOrWhiteSpace(NoweRozpoznanie) ? null : NoweRozpoznanie,
                        Zalecenia = string.IsNullOrWhiteSpace(NoweZalecenia) ? null : NoweZalecenia
                    };
                    db.Wizyty.Add(nowaWizyta);

                    var rezerwacjaWBazie = db.Harmonogramy.Find(AktywnaRezerwacja.Id);
                    if (rezerwacjaWBazie != null) rezerwacjaWBazie.StatusWizyty = "Zrealizowana";

                    db.SaveChanges();

                    AktywnaRezerwacja = null;

                    NowyWywiad = string.Empty;
                    NoweRozpoznanie = string.Empty;
                    NoweZalecenia = string.Empty;

                    MessageBox.Show("Wizyta została pomyślnie zapisana i zakończona.", "Sukces");

                    WeakReferenceMessenger.Default.Send(new WizytaZakonczonaMessage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd");
                }
            }
        }
    }
}
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class GabinetViewModel : ObservableObject
    {
        [ObservableProperty] private Harmonogram? _aktywnaRezerwacja;
        [ObservableProperty] private ObservableCollection<WizytaMedyczna> _historiaWizyt = new();

        public void RozpocznijWizyte(Harmonogram rezerwacja)
        {
            AktywnaRezerwacja = rezerwacja;
            ZaladujHistorie(rezerwacja.ZwierzeId);
        }

        private void ZaladujHistorie(int zwierzeId)
        {
            using var db = new ApplicationDbContext();
            var historia = db.Wizyty
                .Where(w => w.Rezerwacja.ZwierzeId == zwierzeId)
                .OrderByDescending(w => w.DataWizyty)
                .ToList();

            HistoriaWizyt = new ObservableCollection<WizytaMedyczna>(historia);
        }
    }
}
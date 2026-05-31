using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public KlienciPacjenciViewModel KlienciVM { get; } = new();
        public KatalogViewModel KatalogVM { get; } = new();
        public HarmonogramViewModel HarmonogramVM { get; } = new();
        public GabinetViewModel GabinetVM { get; } = new();

        [ObservableProperty]
        private object _aktualnyWidok;

        public MainViewModel()
        {
            AktualnyWidok = HarmonogramVM;

            WeakReferenceMessenger.Default.Register<PrzejdzDoGabinetuMessage>(this, (r, m) =>
            {
                GabinetVM.RozpocznijWizyte(m.Rezerwacja);
                AktualnyWidok = GabinetVM;
            });

            WeakReferenceMessenger.Default.Register<WizytaZakonczonaMessage>(this, (r, m) =>
            {
                AktualnyWidok = HarmonogramVM;
            });
        }

        [RelayCommand]
        private void PokazTerminarz() => AktualnyWidok = HarmonogramVM;

        [RelayCommand]
        private void PokazKlienci() => AktualnyWidok = KlienciVM;

        [RelayCommand]
        private void PokazKatalog() => AktualnyWidok = KatalogVM;
    }
}
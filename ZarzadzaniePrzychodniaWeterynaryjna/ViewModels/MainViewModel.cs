using CommunityToolkit.Mvvm.ComponentModel;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public KlienciPacjenciViewModel KlienciVM { get; } = new();
        public KatalogViewModel KatalogVM { get; } = new();
        public HarmonogramViewModel HarmonogramVM { get; } = new();
    }
}
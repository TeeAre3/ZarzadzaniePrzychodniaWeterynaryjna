using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class CatalogViewModel : ObservableObject
    {
        private readonly CatalogRepository _catalogRepository;

        [ObservableProperty] private ObservableCollection<CatalogItem> _catalogList = new();
        [ObservableProperty] private string _newItemName = string.Empty;
        [ObservableProperty] private int _newItemTypeIndex = 0;
        [ObservableProperty] private decimal? _newItemPrice;
        [ObservableProperty] private decimal? _newItemVAT = 23;
        [ObservableProperty] private string _newItemUnit = string.Empty;
        [ObservableProperty] private int _newItemDurationMin = 15;

        public CatalogViewModel(CatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
            LoadCatalog();
        }

        private void LoadCatalog()
        {
            CatalogList = new ObservableCollection<CatalogItem>(_catalogRepository.GetCatalog());
        }

        [RelayCommand]
        private void AddCatalogItem()
        {
            if (string.IsNullOrWhiteSpace(NewItemName) || string.IsNullOrWhiteSpace(NewItemUnit)) { MessageBox.Show("Uzupełnij Nazwę i Jednostkę!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if ((NewItemPrice ?? 0) < 0 || (NewItemVAT ?? 0) < 0) { MessageBox.Show("Cena/VAT ujemne!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            var newItem = new CatalogItem
            {
                Name = NewItemName,
                ItemType = NewItemTypeIndex switch { 0 => "Usługa", 1 => "Towar", 2 => "Lek", _ => "Inne" },
                Price = NewItemPrice ?? 0,
                VAT = NewItemVAT ?? 0,
                Unit = NewItemUnit,
                DurationMin = NewItemDurationMin
            };

            try
            {
                _catalogRepository.AddItem(newItem);
                WeakReferenceMessenger.Default.Send(new CatalogChangedMessage());
                MessageBox.Show("Dodano pozycję!", "Sukces");
            }
            catch (System.Exception ex) { MessageBox.Show($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }

            LoadCatalog();
            NewItemName = NewItemUnit = string.Empty; NewItemPrice = null; NewItemDurationMin = 15;
        }
    }
}
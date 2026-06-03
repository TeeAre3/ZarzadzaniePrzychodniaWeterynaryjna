using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;
using ZarzadzaniePrzychodniaWeterynaryjna.Services;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class CatalogViewModel : ObservableObject
    {
        private readonly CatalogRepository _catalogRepository;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private ObservableCollection<CatalogItem> _catalogList = [];
        [ObservableProperty] private string _newItemName = string.Empty;
        [ObservableProperty] private int _newItemTypeIndex = 0;
        [ObservableProperty] private decimal? _newItemPrice;
        [ObservableProperty] private decimal? _newItemVAT = 23;
        [ObservableProperty] private string _newItemUnit = string.Empty;
        [ObservableProperty] private int _newItemDurationMin = 15;

        public CatalogViewModel(CatalogRepository catalogRepository, IDialogService dialogService)
        {
            _catalogRepository = catalogRepository;
            _dialogService = dialogService;
            LoadCatalog();
        }

        private void LoadCatalog()
        {
            CatalogList = new ObservableCollection<CatalogItem>(_catalogRepository.GetCatalog());
        }

        [RelayCommand]
        private void AddCatalogItem()
        {
            if (string.IsNullOrWhiteSpace(NewItemName) || (NewItemTypeIndex != 0 && string.IsNullOrWhiteSpace(NewItemUnit)))
            {
                _dialogService.ShowError("Uzupełnij Nazwę i Jednostkę (jeśli dodajesz lek/towar)!", "Błąd");
                return;
            }

            if (NewItemPrice == null || NewItemVAT == null)
            {
                _dialogService.ShowError("Wprowadź poprawną liczbę dla Ceny i VAT! Wpisywanie tekstu jest niedozwolone.", "Błąd");
                return;
            }

            if (NewItemPrice < 0 || NewItemVAT < 0)
            {
                _dialogService.ShowError("Cena i VAT nie mogą być ujemne!", "Błąd");
                return;
            }

            var newItem = new CatalogItem
            {
                Name = NewItemName,
                ItemType = NewItemTypeIndex switch { 0 => "Usługa", 1 => "Towar", 2 => "Lek", _ => "Inne" },
                Price = NewItemPrice.Value,
                VAT = NewItemVAT.Value,
                Unit = NewItemTypeIndex == 0 ? "usł." : NewItemUnit,
                DurationMin = NewItemTypeIndex == 0 ? NewItemDurationMin : 0
            };

            try
            {
                _catalogRepository.AddItem(newItem);
                WeakReferenceMessenger.Default.Send(new CatalogChangedMessage());
                _dialogService.ShowInformation("Dodano pozycję!", "Sukces");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd");
                return;
            }

            LoadCatalog();
            NewItemName = NewItemUnit = string.Empty;
            NewItemPrice = null;
            NewItemDurationMin = 15;
        }

        [RelayCommand]
        private void RemoveCatalogItem(CatalogItem item)
        {
            if (item == null) return;

            if (_dialogService.AskQuestion($"Czy na pewno chcesz usunąć pozycję '{item.Name}' z katalogu?", "Potwierdzenie"))
            {
                try
                {
                    _catalogRepository.RemoveItem(item);
                    LoadCatalog();
                    WeakReferenceMessenger.Default.Send(new CatalogChangedMessage());
                }
                catch (System.Exception ex)
                {
                    _dialogService.ShowError(ex.Message, "Błąd usuwania");
                }
            }
        }
    }
}
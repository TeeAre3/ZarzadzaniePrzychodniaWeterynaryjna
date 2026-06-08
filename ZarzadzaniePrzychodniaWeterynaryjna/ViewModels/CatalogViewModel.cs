using System;
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
                _dialogService.ShowError("Uzupełnij Nazwę i Jednostkę (jeśli dodajesz lek/towar)!", "Błąd"); return;
            }
            if (NewItemPrice == null || NewItemVAT == null)
            {
                _dialogService.ShowError("Wprowadź poprawną liczbę dla Ceny i VAT! Wpisywanie tekstu jest niedozwolone.", "Błąd"); return;
            }
            if (NewItemPrice < 0 || NewItemVAT < 0)
            {
                _dialogService.ShowError("Cena i VAT nie mogą być ujemne!", "Błąd"); return;
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

            ExecuteSafeOperation(
                dbAction: () => _catalogRepository.AddItem(newItem),
                onSuccess: () =>
                {
                    LoadCatalog();
                    WeakReferenceMessenger.Default.Send(new CatalogChangedMessage());
                    NewItemName = NewItemUnit = string.Empty;
                    NewItemPrice = null; NewItemDurationMin = 15;
                },
                successMsg: "Dodano pozycję!"
            );
        }

        [RelayCommand]
        private void RemoveCatalogItem(CatalogItem item) =>
            RemoveEntity(item, $"Czy na pewno chcesz usunąć pozycję '{item?.Name}' z katalogu?",
                removeAction: () => _catalogRepository.RemoveItem(item!),
                onSuccess: () => { LoadCatalog(); WeakReferenceMessenger.Default.Send(new CatalogChangedMessage()); },
                errorMsg: "Nie można usunąć tej pozycji. Prawdopodobnie jest używana w historii wizyt."
            );

        private void ExecuteSafeOperation(Action dbAction, Action onSuccess, string? successMsg = null, string? errorMsg = null)
        {
            try
            {
                dbAction();
                if (!string.IsNullOrEmpty(successMsg)) _dialogService.ShowInformation(successMsg, "Sukces");
                onSuccess?.Invoke();
            }
            catch (Exception ex) { _dialogService.ShowError(errorMsg ?? $"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); }
        }

        private void RemoveEntity<T>(T entity, string promptMessage, Action removeAction, Action onSuccess, string errorMsg)
        {
            if (entity == null) return;
            if (_dialogService.AskQuestion(promptMessage, "Potwierdzenie"))
                ExecuteSafeOperation(removeAction, onSuccess, null, errorMsg);
        }
    }
}
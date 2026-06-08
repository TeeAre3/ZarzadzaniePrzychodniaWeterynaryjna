using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;
using ZarzadzaniePrzychodniaWeterynaryjna.Services;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class ClientsPatientsViewModel : ObservableValidator
    {
        private readonly ClientRepository _clientRepository;
        private readonly PatientRepository _patientRepository;
        private readonly IDialogService _dialogService;

        [ObservableProperty] private ObservableCollection<Client> _clientsList = [];
        [ObservableProperty] private ICollectionView _clientsView = null!;

        [ObservableProperty] private string _searchQuery = string.Empty;
        partial void OnSearchQueryChanged(string value) => ClientsView?.Refresh();

        [ObservableProperty] private int _selectedClientType = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(ClientsPatientsViewModel), nameof(ValidateFirstName))]
        private string _newFirstName = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(ClientsPatientsViewModel), nameof(ValidateLastName))]
        private string _newLastName = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(ClientsPatientsViewModel), nameof(ValidateCompanyName))]
        private string _newCompanyName = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Wprowadź poprawny numer telefonu (np. 123456789)!")]
        private string _newPhoneNumber = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email jest wymagany!")]
        [EmailAddress(ErrorMessage = "Wprowadź poprawny adres email (np. jan@kowalski.pl)!")]
        private string _newEmail = string.Empty;

        [ObservableProperty] private ObservableCollection<Patient> _patientsList = [];
        [ObservableProperty] private Client? _selectedClient;

        partial void OnSelectedClientChanged(Client? value) => _ = LoadPatientsAsync();

        [ObservableProperty] private Patient? _selectedPatient;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Imię pacjenta jest wymagane!")]
        private string _newPatientName = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Gatunek pacjenta jest wymagany!")]
        private string _newPatientSpecies = string.Empty;

        [ObservableProperty] private string _newPatientBreed = string.Empty;
        [ObservableProperty] private int _newPatientGenderIndex = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Waga jest wymagana!")]
        [RegularExpression(@"^[0-9]+([.,][0-9]+)?$", ErrorMessage = "Wprowadź poprawną wagę (np. 2.5)!")]
        private string _newPatientWeight = string.Empty;

        public ClientsPatientsViewModel(ClientRepository clientRepository, PatientRepository patientRepository, IDialogService dialogService)
        {
            _clientRepository = clientRepository;
            _patientRepository = patientRepository;
            _dialogService = dialogService;
            _ = LoadClientsAsync();
        }

        public static ValidationResult? ValidateFirstName(string name, ValidationContext context)
        {
            var vm = (ClientsPatientsViewModel)context.ObjectInstance;
            if ((vm.SelectedClientType == 0 || vm.SelectedClientType == 2) && string.IsNullOrWhiteSpace(name))
                return new ValidationResult("Imię jest wymagane!");
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateLastName(string name, ValidationContext context)
        {
            var vm = (ClientsPatientsViewModel)context.ObjectInstance;
            if ((vm.SelectedClientType == 0 || vm.SelectedClientType == 2) && string.IsNullOrWhiteSpace(name))
                return new ValidationResult("Nazwisko jest wymagane!");
            return ValidationResult.Success;
        }

        public static ValidationResult? ValidateCompanyName(string name, ValidationContext context)
        {
            var vm = (ClientsPatientsViewModel)context.ObjectInstance;
            if ((vm.SelectedClientType == 1 || vm.SelectedClientType == 2) && string.IsNullOrWhiteSpace(name))
                return new ValidationResult("Nazwa firmy jest wymagana!");
            return ValidationResult.Success;
        }

        private async Task LoadClientsAsync()
        {
            var lista = await _clientRepository.GetClientsAsync();
            ClientsList = new ObservableCollection<Client>(lista);
            ClientsView = CollectionViewSource.GetDefaultView(ClientsList);
            ClientsView.Filter = FilterClients;
        }

        private async Task LoadPatientsAsync()
        {
            if (SelectedClient == null) { PatientsList.Clear(); return; }
            var lista = await _patientRepository.GetPatientsAsync(SelectedClient.Id);

            PatientsList.Clear();
            foreach (var patient in lista)
            {
                PatientsList.Add(patient);
            }
        }

        private bool FilterClients(object obj)
        {
            if (obj is not Client w) return false;
            if (string.IsNullOrWhiteSpace(SearchQuery)) return true;

            return (w.FirstName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true) ||
                   (w.LastName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true) ||
                   (w.CompanyName?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true) ||
                   (w.PhoneNumber?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true);
        }

        [RelayCommand]
        private void AddClient()
        {
            ClearErrors();
            ValidateProperty(NewFirstName, nameof(NewFirstName));
            ValidateProperty(NewLastName, nameof(NewLastName));
            ValidateProperty(NewCompanyName, nameof(NewCompanyName));
            ValidateProperty(NewPhoneNumber, nameof(NewPhoneNumber));
            ValidateProperty(NewEmail, nameof(NewEmail));

            if (CheckValidationErrors("Błędy walidacji formularza klienta")) return;

            var newClient = new Client
            {
                FirstName = string.IsNullOrWhiteSpace(NewFirstName) ? null : NewFirstName,
                LastName = string.IsNullOrWhiteSpace(NewLastName) ? null : NewLastName,
                CompanyName = string.IsNullOrWhiteSpace(NewCompanyName) ? null : NewCompanyName,
                PhoneNumber = string.IsNullOrWhiteSpace(NewPhoneNumber) ? null : NewPhoneNumber,
                Email = NewEmail,
                RegistrationDate = DateTime.Now
            };

            ExecuteSafeOperation(
                dbAction: () => _clientRepository.AddClient(newClient),
                onSuccess: () =>
                {
                    _ = LoadClientsAsync();
                    WeakReferenceMessenger.Default.Send(new ClientChangedMessage());
                    NewFirstName = NewLastName = NewCompanyName = NewPhoneNumber = NewEmail = string.Empty;
                    ClearErrors();
                },
                successMsg: "Dodano klienta!"
            );
        }

        [RelayCommand]
        private void AddPatient()
        {
            if (SelectedClient == null) { _dialogService.ShowInformation("Wybierz właściciela!", "Informacja"); return; }

            ClearErrors();
            ValidateProperty(NewPatientName, nameof(NewPatientName));
            ValidateProperty(NewPatientSpecies, nameof(NewPatientSpecies));
            ValidateProperty(NewPatientWeight, nameof(NewPatientWeight));

            if (CheckValidationErrors("Błędy walidacji formularza pacjenta")) return;

            string cleanWeight = NewPatientWeight.Replace(",", ".");
            if (!decimal.TryParse(cleanWeight, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal parsedWeight) || parsedWeight <= 0)
            {
                _dialogService.ShowError("Waga musi być liczbą większą od zera!", "Błąd walidacji");
                return;
            }

            var newPatient = new Patient
            {
                ClientId = SelectedClient.Id,
                Name = NewPatientName,
                Species = NewPatientSpecies,
                Breed = string.IsNullOrWhiteSpace(NewPatientBreed) ? null : NewPatientBreed,
                Gender = NewPatientGenderIndex == 0 ? "S" : "M",
                Weight = parsedWeight
            };

            ExecuteSafeOperation(
                dbAction: () => _patientRepository.AddPatient(newPatient),
                onSuccess: () =>
                {
                    PatientsList.Add(newPatient);

                    WeakReferenceMessenger.Default.Send(new PatientChangedMessage());
                    NewPatientName = NewPatientSpecies = NewPatientBreed = string.Empty;
                    NewPatientWeight = string.Empty;
                    ClearErrors();
                },
                successMsg: "Dodano pacjenta!"
            );
        }

        [RelayCommand]
        private void EditClient()
        {
            if (!_clientRepository.HasChanges()) { _dialogService.ShowInformation("Nie wprowadzono żadnych zmian w danych klientów", "Informacja"); return; }

            ExecuteSafeOperation(
                dbAction: () => _clientRepository.SaveAllChanges(),
                onSuccess: () => { _ = LoadClientsAsync(); WeakReferenceMessenger.Default.Send(new ClientChangedMessage()); },
                successMsg: "Zapisano wszystkie zmiany w klientach!"
            );
        }

        [RelayCommand]
        private void EditPatient()
        {
            if (!_patientRepository.HasChanges()) { _dialogService.ShowInformation("Nie wprowadzono żadnych zmian w danych pacjentów", "Informacja"); return; }

            ExecuteSafeOperation(
                dbAction: () => _patientRepository.SaveAllChanges(),
                onSuccess: () => { _ = LoadPatientsAsync(); WeakReferenceMessenger.Default.Send(new PatientChangedMessage()); },
                successMsg: "Zapisano wszystkie zmiany w pacjentach!"
            );
        }

        [RelayCommand]
        private void RemoveClient(Client? client) =>
            RemoveEntity(client, $"klienta {client?.DisplayName}",
                removeAction: () => { _clientRepository.RemoveClient(client!); if (SelectedClient?.Id == client?.Id) SelectedClient = null; },
                onSuccess: () => { _ = LoadClientsAsync(); WeakReferenceMessenger.Default.Send(new ClientChangedMessage()); },
                errorMsg: "Nie można usunąć tego klienta, ponieważ ma przypisane zwierzęta w systemie. Najpierw usuń jego zwierzęta."
            );

        [RelayCommand]
        private void RemovePatient(Patient? patient) =>
            RemoveEntity(patient, $"pacjenta {patient?.Name}",
                removeAction: () => _patientRepository.RemovePatient(patient!),
                onSuccess: () => { _ = LoadPatientsAsync(); WeakReferenceMessenger.Default.Send(new PatientChangedMessage()); },
                errorMsg: "Nie można usunąć pacjenta, ponieważ ma zapisaną historię wizyt."
            );

        private void ExecuteSafeOperation(Action dbAction, Action onSuccess, string? successMsg = null, string? errorMsg = null)
        {
            try
            {
                dbAction();
                if (!string.IsNullOrEmpty(successMsg))
                    _dialogService.ShowInformation(successMsg, "Sukces");

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(errorMsg ?? $"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd");
            }
        }

        private void RemoveEntity<T>(T? entity, string promptName, Action removeAction, Action onSuccess, string errorMsg)
        {
            if (entity == null) return;
            if (_dialogService.AskQuestion($"Czy na pewno chcesz usunąć {promptName}?", "Potwierdzenie"))
            {
                ExecuteSafeOperation(removeAction, onSuccess, null, errorMsg);
            }
        }

        private bool CheckValidationErrors(string errorTitle)
        {
            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
                _dialogService.ShowError(errors, errorTitle);
                return true;
            }
            return false;
        }
    }
}
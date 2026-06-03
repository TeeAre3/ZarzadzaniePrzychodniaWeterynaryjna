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
        [Range(0.01, 1000.0, ErrorMessage = "Waga musi być większa od 0!")]
        private decimal? _newPatientWeight;

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
            PatientsList = new ObservableCollection<Patient>(lista);
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


            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
                _dialogService.ShowError(errors, "Błędy walidacji formularza klienta");
                return;
            }

            var newClient = new Client
            {
                FirstName = string.IsNullOrWhiteSpace(NewFirstName) ? null : NewFirstName,
                LastName = string.IsNullOrWhiteSpace(NewLastName) ? null : NewLastName,
                CompanyName = string.IsNullOrWhiteSpace(NewCompanyName) ? null : NewCompanyName,
                PhoneNumber = string.IsNullOrWhiteSpace(NewPhoneNumber) ? null : NewPhoneNumber,
                Email = NewEmail,
                RegistrationDate = DateTime.Now
            };

            try
            {
                _clientRepository.AddClient(newClient);
                _dialogService.ShowInformation("Dodano klienta!", "Sukces");
                WeakReferenceMessenger.Default.Send(new ClientChangedMessage());
            }
            catch (Exception ex) { _dialogService.ShowError($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }

            _ = LoadClientsAsync();
            NewFirstName = NewLastName = NewCompanyName = NewPhoneNumber = NewEmail = string.Empty;
            ClearErrors();
        }

        [RelayCommand]
        private void AddPatient()
        {
            if (SelectedClient == null) { _dialogService.ShowInformation("Wybierz właściciela!", "Informacja"); return; }
            
            ClearErrors();

            ValidateProperty(NewPatientName, nameof(NewPatientName));
            ValidateProperty(NewPatientSpecies, nameof(NewPatientSpecies));
            ValidateProperty(NewPatientWeight, nameof(NewPatientWeight));

            if (HasErrors)
            {
                var errors = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
                _dialogService.ShowError(errors, "Błędy walidacji formularza pacjenta");
                return;
            }

            var newPatient = new Patient
            {
                ClientId = SelectedClient.Id,
                Name = NewPatientName,
                Species = NewPatientSpecies,
                Breed = string.IsNullOrWhiteSpace(NewPatientBreed) ? null : NewPatientBreed,
                Gender = NewPatientGenderIndex == 0 ? "S" : "M",
                Weight = NewPatientWeight
            };

            try
            {
                _patientRepository.AddPatient(newPatient);
                _dialogService.ShowInformation("Dodano pacjenta!", "Sukces");
                WeakReferenceMessenger.Default.Send(new PatientChangedMessage());
            }
            catch (Exception ex) { _dialogService.ShowError($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }

            _ = LoadPatientsAsync();
            NewPatientName = NewPatientSpecies = NewPatientBreed = string.Empty;
            NewPatientWeight = null;
            ClearErrors();
        }

        [RelayCommand]
        private void EditClient()
        {
            if (!_clientRepository.HasChanges())
            {
                _dialogService.ShowInformation("Nie wprowadzono żadnych zmian w danych klientów", "Informacja");
                return;
            }

            try
            {
                _clientRepository.SaveAllChanges();
                _dialogService.ShowInformation("Zapisano wszystkie zmiany w klientach!", "Sukces");
                _ = LoadClientsAsync();
                WeakReferenceMessenger.Default.Send(new ClientChangedMessage());
            }
            catch (Exception ex) { _dialogService.ShowError($"Błąd zapisu: {ex.Message}", "Błąd"); }
        }

        [RelayCommand]
        private void EditPatient()
        {
            if (!_patientRepository.HasChanges())
            {
                _dialogService.ShowInformation("Nie wprowadzono żadnych zmian w danych pacjentów", "Informacja");
                return;
            }

            try
            {
                _patientRepository.SaveAllChanges();
                _dialogService.ShowInformation("Zapisano wszystkie zmiany w pacjentach!", "Sukces");
                _ = LoadPatientsAsync();
                WeakReferenceMessenger.Default.Send(new PatientChangedMessage());
            }
            catch (Exception ex) { _dialogService.ShowError($"Błąd zapisu: {ex.Message}", "Błąd"); }
        }

        [RelayCommand]
        private void RemoveClient(Client client)
        {
            if (client == null) return;
            if (_dialogService.AskQuestion($"Czy na pewno chcesz usunąć klienta {client.DisplayName}?", "Potwierdzenie"))
            {
                try
                {
                    _clientRepository.RemoveClient(client);
                    if (SelectedClient?.Id == client.Id) SelectedClient = null;
                    _ = LoadClientsAsync();
                    WeakReferenceMessenger.Default.Send(new ClientChangedMessage());
                }
                catch (Exception)
                {
                    _dialogService.ShowError("Nie można usunąć tego klienta, ponieważ ma przypisane zwierzęta w systemie. Najpierw usuń jego zwierzęta.", "Błąd usuwania");
                }
            }
        }

        [RelayCommand]
        private void RemovePatient(Patient patient)
        {
            if (patient == null) return;
            if (_dialogService.AskQuestion($"Czy na pewno chcesz usunąć pacjenta {patient.Name}?", "Potwierdzenie"))
            {
                try
                {
                    _patientRepository.RemovePatient(patient);
                    _ = LoadPatientsAsync();
                    WeakReferenceMessenger.Default.Send(new PatientChangedMessage());
                }
                catch (Exception)
                {
                    _dialogService.ShowError("Nie można usunąć pacjenta, ponieważ ma zapisaną historię wizyt.", "Błąd usuwania");
                }
            }
        }
    }
}
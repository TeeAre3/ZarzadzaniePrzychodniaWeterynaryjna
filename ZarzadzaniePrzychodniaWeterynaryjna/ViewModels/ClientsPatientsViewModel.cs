using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class ClientsPatientsViewModel : ObservableObject
    {
        private readonly ClientRepository _clientRepository;

        [ObservableProperty] private ObservableCollection<Client> _clientsList = new();
        [ObservableProperty] private ICollectionView _clientsView = null!;

        [ObservableProperty] private string _searchQuery = string.Empty;
        partial void OnSearchQueryChanged(string value) => ClientsView?.Refresh();

        [ObservableProperty] private int _selectedClientType = 0;
        [ObservableProperty] private string _newFirstName = string.Empty;
        [ObservableProperty] private string _newLastName = string.Empty;
        [ObservableProperty] private string _newCompanyName = string.Empty;
        [ObservableProperty] private string _newPhoneNumber = string.Empty;
        [ObservableProperty] private string _newEmail = string.Empty;

        [ObservableProperty] private ObservableCollection<Patient> _patientsList = new();
        [ObservableProperty] private Client? _selectedClient;

        partial void OnSelectedClientChanged(Client? value) => _ = LoadPatientsAsync();

        [ObservableProperty] private Patient? _selectedPatient;

        [ObservableProperty] private string _newPatientName = string.Empty;
        [ObservableProperty] private string _newPatientSpecies = string.Empty;
        [ObservableProperty] private string _newPatientBreed = string.Empty;
        [ObservableProperty] private int _newPatientGenderIndex = 0;
        [ObservableProperty] private decimal? _newPatientWeight;

        public ClientsPatientsViewModel(ClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            _ = LoadClientsAsync();
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
            var lista = await _clientRepository.GetPatientsAsync(SelectedClient.Id);
            PatientsList = new ObservableCollection<Patient>(lista);
        }

        private bool FilterClients(object obj)
        {
            if (obj is not Client w) return false;
            if (string.IsNullOrWhiteSpace(SearchQuery)) return true;

            var search = SearchQuery.ToLower();
            return (w.FirstName?.ToLower().Contains(search) == true) ||
                   (w.LastName?.ToLower().Contains(search) == true) ||
                   (w.CompanyName?.ToLower().Contains(search) == true) ||
                   (w.PhoneNumber?.Contains(search) == true);
        }

        [RelayCommand]
        private void AddClient()
        {
            if (string.IsNullOrWhiteSpace(NewEmail)) { MessageBox.Show("Email jest wymagany!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if ((SelectedClientType == 0 || SelectedClientType == 2) && (string.IsNullOrWhiteSpace(NewFirstName) || string.IsNullOrWhiteSpace(NewLastName))) { MessageBox.Show("Uzupełnij Imię i Nazwisko!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if ((SelectedClientType == 1 || SelectedClientType == 2) && string.IsNullOrWhiteSpace(NewCompanyName)) { MessageBox.Show("Uzupełnij Nazwę Firmy!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            if (!System.Text.RegularExpressions.Regex.IsMatch(NewEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Wprowadź poprawny adres email (np. jan@kowalski.pl)!", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(NewPhoneNumber))
            {
                var cleanPhone = NewPhoneNumber.Replace(" ", "").Replace("-", "");

                if (!System.Text.RegularExpressions.Regex.IsMatch(cleanPhone, @"^\+?[0-9]{9,15}$"))
                {
                    MessageBox.Show("Wprowadź poprawny numer telefonu (np. 123456789 lub +48 123 456 789)!", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var newClient = new Client
            {
                FirstName = string.IsNullOrWhiteSpace(NewFirstName) ? null : NewFirstName,
                LastName = string.IsNullOrWhiteSpace(NewLastName) ? null : NewLastName,
                CompanyName = string.IsNullOrWhiteSpace(NewCompanyName) ? null : NewCompanyName,
                PhoneNumber = string.IsNullOrWhiteSpace(NewPhoneNumber) ? null : NewPhoneNumber,
                Email = NewEmail,
                RegistrationDate = System.DateTime.Now
            };

            try
            {
                _clientRepository.AddClient(newClient);
                MessageBox.Show("Dodano klienta!", "Sukces");
                WeakReferenceMessenger.Default.Send(new ClientChangedMessage());
            }
            catch (System.Exception ex) { MessageBox.Show($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }

            _ = LoadClientsAsync();
            NewFirstName = NewLastName = NewCompanyName = NewPhoneNumber = NewEmail = string.Empty;
        }

        [RelayCommand]
        private void AddPatient()
        {
            if (SelectedClient == null) { MessageBox.Show("Wybierz właściciela!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information); return; }
            if (string.IsNullOrWhiteSpace(NewPatientName) || string.IsNullOrWhiteSpace(NewPatientSpecies)) { MessageBox.Show("Uzupełnij Imię i Gatunek!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }
            if (NewPatientWeight <= 0) { MessageBox.Show("Waga musi być > 0!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

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
                _clientRepository.AddPatient(newPatient);
                MessageBox.Show("Dodano pacjenta!", "Sukces");
                WeakReferenceMessenger.Default.Send(new PatientChangedMessage());
            }
            catch (System.Exception ex) { MessageBox.Show($"Błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); return; }

            _ = LoadPatientsAsync();
            NewPatientName = NewPatientSpecies = NewPatientBreed = string.Empty; NewPatientWeight = null;
        }

        [RelayCommand]
        private void EditClient()
        {
            if(!_clientRepository.HasChanges())
            {
                MessageBox.Show("Nie wprowadzono żadnych zmian w danych klientów", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var client in ClientsList)
            {
                if (string.IsNullOrWhiteSpace(client.Email) || !System.Text.RegularExpressions.Regex.IsMatch(client.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MessageBox.Show($"Klient {client.DisplayName} ma niepoprawny adres email!\nPopraw to w tabeli przed zapisem.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(client.PhoneNumber))
                {
                    var cleanPhone = client.PhoneNumber.Replace(" ", "").Replace("-", "");
                    if (!System.Text.RegularExpressions.Regex.IsMatch(cleanPhone, @"^\+?[0-9]{9,15}$"))
                    {
                        MessageBox.Show($"Klient {client.DisplayName} ma niepoprawny numer telefonu!\nPopraw to w tabeli przed zapisem.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return; 
                    }
                }
            }

            try
            {
                _clientRepository.SaveAllChanges();
                MessageBox.Show("Zapisano wszystkie zmiany w klientach!", "Sukces");
                _ = LoadClientsAsync();
                WeakReferenceMessenger.Default.Send(new ClientChangedMessage());
            }
            catch (System.Exception ex) { MessageBox.Show($"Błąd zapisu: {ex.Message}"); }
        }

        [RelayCommand]
        private void EditPatient()
        {
            if (!_clientRepository.HasChanges())
            {
                MessageBox.Show("Nie wprowadzono żadnych zmian w danych pacjentów", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _clientRepository.SaveAllChanges();
                MessageBox.Show("Zapisano wszystkie zmiany w pacjentach!", "Sukces");
                _ = LoadPatientsAsync();
                WeakReferenceMessenger.Default.Send(new PatientChangedMessage());
            }
            catch (System.Exception ex) { MessageBox.Show($"Błąd zapisu: {ex.Message}"); }
        }

        [RelayCommand]
        private void RemoveClient(Client client)
        {
            if (client == null) return;
            if (MessageBox.Show($"Czy na pewno chcesz usunąć klienta {client.DisplayName}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _clientRepository.RemoveClient(client);
                    if (SelectedClient?.Id == client.Id) SelectedClient = null; // Czyszczenie zaznaczenia
                    _ = LoadClientsAsync();
                    WeakReferenceMessenger.Default.Send(new ClientChangedMessage());
                }
                catch (System.Exception) { MessageBox.Show("Nie można usunąć tego klienta, ponieważ ma przypisane zwierzęta w systemie. Najpierw usuń jego zwierzęta.", "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Warning); }
            }
        }

        [RelayCommand]
        private void RemovePatient(Patient patient)
        {
            if (patient == null) return;
            if (MessageBox.Show($"Czy na pewno chcesz usunąć pacjenta {patient.Name}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _clientRepository.RemovePatient(patient);
                    _ = LoadPatientsAsync();
                    WeakReferenceMessenger.Default.Send(new PatientChangedMessage());
                }
                catch (System.Exception) { MessageBox.Show("Nie można usunąć pacjenta, ponieważ ma zapisaną historię wizyt.", "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Warning); }
            }
        }
    }
}
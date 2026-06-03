using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;
using ZarzadzaniePrzychodniaWeterynaryjna.Helpers;
using ZarzadzaniePrzychodniaWeterynaryjna.Services;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ClientRepository _clientRepository;
        private readonly PatientRepository _patientRepository;
        private readonly CatalogRepository _catalogRepository;
        private readonly IDialogService _dialogService;

        private static readonly ObservableCollection<Appointment> value = [];
        [ObservableProperty] private ObservableCollection<Appointment> _appointmentList = value;
        [ObservableProperty] private ObservableCollection<Client> _clientsList = [];
        [ObservableProperty] private ObservableCollection<Patient> _patientsList = [];
        [ObservableProperty] private ObservableCollection<CatalogItem> _servicesList = [];

        [ObservableProperty] private Client? _selectedClient;
        partial void OnSelectedClientChanged(Client? value) => _ = LoadPatientsAsync();

        [ObservableProperty] private Patient? _selectedPatient;
        [ObservableProperty] private CatalogItem? _selectedService;
        partial void OnSelectedServiceChanged(CatalogItem? value) => CalculateEstimatedDuration();

        [ObservableProperty] private DateTime _scheduledDate = DateTime.Today;
        [ObservableProperty] private string _scheduledTime = "10:00";
        [ObservableProperty] private int _estimatedDurationMin = 15;
        [ObservableProperty] private Appointment? _selectedAppointment;

        public ScheduleViewModel(
            AppointmentRepository appointmentRepository,
            ClientRepository clientRepository,
            PatientRepository patientRepository,
            CatalogRepository catalogRepository,
            IDialogService dialogService)
        {
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            _patientRepository = patientRepository;
            _catalogRepository = catalogRepository;
            _dialogService = dialogService;

            _ = LoadClientsAsync();
            LoadServices();
            LoadSchedule();

            WeakReferenceMessenger.Default.Register<CatalogChangedMessage>(this, (r, m) => LoadServices());
            WeakReferenceMessenger.Default.Register<ConsultationEndedMessage>(this, (r, m) => LoadSchedule());
            WeakReferenceMessenger.Default.Register<ClientChangedMessage>(this, (r, m) => _ = LoadClientsAsync());
            WeakReferenceMessenger.Default.Register<PatientChangedMessage>(this, (r, m) =>
            {
                if (SelectedClient != null) _ = LoadPatientsAsync();
            });
        }

        private async Task LoadClientsAsync()
        {
            var lista = await _clientRepository.GetClientsAsync();
            ClientsList = new ObservableCollection<Client>(lista);
        }

        private async Task LoadPatientsAsync()
        {
            if (SelectedClient == null) { PatientsList.Clear(); return; }
            var lista = await _patientRepository.GetPatientsAsync(SelectedClient.Id);
            PatientsList = new ObservableCollection<Patient>(lista);
        }

        private void LoadServices()
        {
            var allItems = _catalogRepository.GetCatalog();
            ServicesList = new ObservableCollection<CatalogItem>(allItems.Where(k => k.ItemType == "Usługa"));
        }

        private void LoadSchedule()
        {
            AppointmentList = new ObservableCollection<Appointment>(_appointmentRepository.GetPlannedAppointments());
        }

        private void CalculateEstimatedDuration()
        {
            if (SelectedService == null) return;
            int duration = SelectedService.DurationMin;
            EstimatedDurationMin = (duration % 15 == 0) ? duration : ((duration / 15) + 1) * 15;
        }

        [RelayCommand]
        private void AddAppointment()
        {
            if (SelectedPatient == null)
            {
                _dialogService.ShowInformation("Wybierz pacjenta!", "Informacja");
                return;
            }

            if (string.IsNullOrWhiteSpace(ScheduledTime) || !ValidationHelper.IsValidTimeFormat(ScheduledTime))
            {
                _dialogService.ShowError("Wprowadź poprawną godzinę w formacie GG:MM (np. 21:37)", "Błąd walidacji");
                return;
            }

            var newAppointment = new Appointment
            {
                PatientId = SelectedPatient.Id,
                Reason = SelectedService?.Name,
                ScheduledDate = ScheduledDate.Date.Add(TimeSpan.Parse(ScheduledTime)),
                EstimatedDurationMin = EstimatedDurationMin,
                Status = "Planowana"
            };

            try
            {
                _appointmentRepository.AddAppointment(newAppointment);
                LoadSchedule();
            }
            catch (Exception ex) { _dialogService.ShowError($"Błąd SQL: {ex.InnerException?.Message ?? ex.Message}", "Błąd"); }
        }

        [RelayCommand]
        private void RemoveAppointment()
        {
            if (SelectedAppointment == null) return;

            try
            {
                _appointmentRepository.RemoveAppointment(SelectedAppointment);
                LoadSchedule();
            }
            catch (Exception ex) { _dialogService.ShowError($"Błąd: {ex.Message}", "Błąd"); }
        }

        [RelayCommand]
        private void StartConsultation()
        {
            if (SelectedAppointment == null) { _dialogService.ShowInformation("Wybierz rezerwację!", "Informacja"); return; }
            WeakReferenceMessenger.Default.Send(new GoToConsultationMessage(SelectedAppointment));
        }
    }
}
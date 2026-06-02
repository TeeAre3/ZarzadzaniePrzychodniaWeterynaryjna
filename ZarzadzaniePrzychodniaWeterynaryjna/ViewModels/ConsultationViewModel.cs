using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ZarzadzaniePrzychodniaWeterynaryjna.Models;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class ConsultationViewModel : ObservableObject
    {
        private readonly ConsultationRepository _consultationRepository;
        private readonly CatalogRepository _catalogRepository;

        [ObservableProperty] private Appointment? _activeAppointment;
        [ObservableProperty] private ObservableCollection<MedicalVisit> _visitHistory = new();

        [ObservableProperty] private string _newInterview = string.Empty;
        [ObservableProperty] private string _newDiagnosis = string.Empty;
        [ObservableProperty] private string _newRecommendations = string.Empty;

        [ObservableProperty] private ObservableCollection<CatalogItem> _availableCatalog = new();
        [ObservableProperty] private ObservableCollection<VisitItem> _addedItems = new();
        [ObservableProperty] private CatalogItem? _selectedCatalogItem;
        [ObservableProperty] private decimal _newQuantity = 1;
        [ObservableProperty] private decimal _totalSum = 0;

        public ConsultationViewModel(ConsultationRepository consultationRepository, CatalogRepository catalogRepository)
        {
            _consultationRepository = consultationRepository;
            _catalogRepository = catalogRepository;
            WeakReferenceMessenger.Default.Register<CatalogChangedMessage>(this, (r, m) => LoadCatalog());
        }

        public void StartConsultation(Appointment appointment)
        {
            ActiveAppointment = appointment;

            NewInterview = string.Empty;
            NewDiagnosis = string.Empty;
            NewRecommendations = string.Empty;

            AddedItems.Clear();
            NewQuantity = 1;
            TotalSum = 0;

            LoadHistory(appointment.PatientId);
            LoadCatalog();
        }

        private void LoadHistory(int patientId)
        {
            var history = _consultationRepository.GetPatientHistory(patientId);
            VisitHistory = new ObservableCollection<MedicalVisit>(history);
        }

        private void LoadCatalog()
        {
            AvailableCatalog = new ObservableCollection<CatalogItem>(_catalogRepository.GetCatalog());
        }

        [RelayCommand]
        private void AddItem()
        {
            if (SelectedCatalogItem == null) return;
            if (NewQuantity <= 0) { MessageBox.Show("Ilość musi być większa od 0.", "Błąd"); return; }

            var newItem = new VisitItem
            {
                CatalogItemId = SelectedCatalogItem.Id,
                CatalogItem = SelectedCatalogItem,
                Quantity = NewQuantity,
                AppliedPrice = SelectedCatalogItem.Price,
                AppliedVAT = SelectedCatalogItem.VAT
            };

            AddedItems.Add(newItem);
            CalculateTotal();

            NewQuantity = 1;
            SelectedCatalogItem = null;
        }

        [RelayCommand]
        private void RemoveItem(VisitItem item)
        {
            if (item != null)
            {
                AddedItems.Remove(item);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            TotalSum = AddedItems.Sum(p => p.AppliedPrice * p.Quantity * (1 + (p.AppliedVAT / 100m)));
        }

        [RelayCommand]
        private void EndConsultation()
        {
            if (ActiveAppointment == null) return;

            if (string.IsNullOrWhiteSpace(NewInterview) && string.IsNullOrWhiteSpace(NewDiagnosis))
            {
                MessageBox.Show("Uzupełnij Wywiad lub Rozpoznanie przed zakończeniem wizyty.", "Informacja");
                return;
            }

            var newVisit = new MedicalVisit
            {
                AppointmentId = ActiveAppointment.Id,
                VisitDate = DateTime.Now,
                Interview = string.IsNullOrWhiteSpace(NewInterview) ? null : NewInterview,
                Diagnosis = string.IsNullOrWhiteSpace(NewDiagnosis) ? null : NewDiagnosis,
                Recommendations = string.IsNullOrWhiteSpace(NewRecommendations) ? null : NewRecommendations
            };

            try
            {
                _consultationRepository.SaveConsultation(newVisit, AddedItems);

                ActiveAppointment = null;
                MessageBox.Show($"Wizyta zakończona. Suma do zapłaty: {TotalSum:N2} zł", "Sukces");

                WeakReferenceMessenger.Default.Send(new ConsultationEndedMessage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.InnerException?.Message ?? ex.Message}", "Błąd");
            }
        }
    }
}
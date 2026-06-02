using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Windows;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ClientsPatientsViewModel ClientsVM { get; } = new();
        public CatalogViewModel CatalogVM { get; } = new();
        public ScheduleViewModel ScheduleVM { get; } = new();
        public ConsultationViewModel ConsultationVM { get; } = new();
        public StatisticsViewModel StatisticsVM { get; } = new();

        [ObservableProperty]
        private object? _currentView;

        public MainViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            CurrentView = ScheduleVM;

            WeakReferenceMessenger.Default.Register<GoToConsultationMessage>(this, (r, m) =>
            {
                ConsultationVM.StartConsultation(m.Appointment);
                CurrentView = ConsultationVM;
            });

            WeakReferenceMessenger.Default.Register<ConsultationEndedMessage>(this, (r, m) =>
            {
                CurrentView = ScheduleVM;
            });
        }

        [RelayCommand] private void ShowSchedule() => CurrentView = ScheduleVM;
        [RelayCommand] private void ShowClients() => CurrentView = ClientsVM;
        [RelayCommand] private void ShowCatalog() => CurrentView = CatalogVM;
        [RelayCommand] private void ShowStatistics() => CurrentView = StatisticsVM;
    }
}
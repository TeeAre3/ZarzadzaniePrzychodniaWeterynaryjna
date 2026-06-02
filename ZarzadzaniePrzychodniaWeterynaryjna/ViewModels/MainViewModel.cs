using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.Windows;

namespace ZarzadzaniePrzychodniaWeterynaryjna.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ClientsPatientsViewModel ClientsVM { get; }
        public CatalogViewModel CatalogVM { get; }
        public ScheduleViewModel ScheduleVM { get; }
        public ConsultationViewModel ConsultationVM { get; }
        public StatisticsViewModel StatisticsVM { get; }

        [ObservableProperty]
        private object? _currentView;

        public MainViewModel(
            ClientsPatientsViewModel clientsVM,
            CatalogViewModel catalogVM,
            ScheduleViewModel scheduleVM,
            ConsultationViewModel consultationVM,
            StatisticsViewModel statisticsVM)
        {
            ClientsVM = clientsVM;
            CatalogVM = catalogVM;
            ScheduleVM = scheduleVM;
            ConsultationVM = consultationVM;
            StatisticsVM = statisticsVM;

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
using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;
using ZarzadzaniePrzychodniaWeterynaryjna.ViewModels;

namespace ZarzadzaniePrzychodniaWeterynaryjna
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>();

            services.AddTransient<AppointmentRepository>();
            services.AddTransient<CatalogRepository>();
            services.AddTransient<ClientRepository>();
            services.AddTransient<ConsultationRepository>();
            services.AddTransient<StatisticsRepository>();

            services.AddTransient<ClientsPatientsViewModel>();
            services.AddTransient<CatalogViewModel>();
            services.AddTransient<ScheduleViewModel>();
            services.AddTransient<ConsultationViewModel>();
            services.AddTransient<StatisticsViewModel>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
            
            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

        }
    }

}

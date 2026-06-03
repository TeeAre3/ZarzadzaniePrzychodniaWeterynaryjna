using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ZarzadzaniePrzychodniaWeterynaryjna.Data;
using ZarzadzaniePrzychodniaWeterynaryjna.Repositories;
using ZarzadzaniePrzychodniaWeterynaryjna.ViewModels;
using ZarzadzaniePrzychodniaWeterynaryjna.Services;

namespace ZarzadzaniePrzychodniaWeterynaryjna
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")),
                ServiceLifetime.Transient); 

            services.AddSingleton<IDialogService, DialogService>();

            services.AddTransient<AppointmentRepository>();
            services.AddTransient<CatalogRepository>();
            services.AddTransient<ClientRepository>();
            services.AddTransient<PatientRepository>();
            services.AddTransient<ConsultationRepository>();
            services.AddTransient<StatisticsRepository>();

            services.AddSingleton<ClientsPatientsViewModel>();
            services.AddSingleton<CatalogViewModel>();
            services.AddSingleton<ScheduleViewModel>();
            services.AddSingleton<ConsultationViewModel>();
            services.AddSingleton<StatisticsViewModel>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
using System.Windows;

using ClipThief.Ui.Extensions;
using ClipThief.Ui.Factories;
using ClipThief.Ui.Services;
using ClipThief.Ui.ViewModels;
using ClipThief.Ui.Views;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClipThief.Ui
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            host = CreateHostBuilder().Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureAppConfiguration(ConfigureAppConfig)
                       .ConfigureServices(ConfigureServices);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await host.StopAsync().ConfigureAwait(false);
            host.Dispose();

            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            host.Start();

            var gestureService = host.Services.GetRequiredService<IGestureService>();
            ObservableExtensions.GestureService = gestureService;

            // set up window
            Window window = host.Services.GetRequiredService<ApplicationView>();
            var viewModel = host.Services.GetRequiredService<ApplicationViewModel>();
            window.DataContext = viewModel;
            window.Show();

            base.OnStartup(e);
        }

        private static void ConfigureAppConfig(IConfigurationBuilder c)
        {
            c.AddJsonFile("appsettings.json");
            c.AddEnvironmentVariables();
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<ApplicationView>();
            services.AddSingleton<ApplicationViewModel>();
            services.AddSingleton<IGestureService, GestureService>();
            services.AddSingleton<IApplicationService, ApplicationService>();
            services.AddSingleton<IFormatSelectionViewModelFactory, FormatSelectionViewModelFactory>();
            services.AddSingleton<IVideoDownloadService, YoutubeDownloadService>();
            services.AddTransient<IDownloadViewModel, DownloadViewModel>();
            services.AddTransient<IVideoFormatSelectionViewModel, VideoFormatSelectionViewModel>();
        }
    }
}
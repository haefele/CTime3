using System.Windows;
using System.Windows.Threading;
using CTime3.Apps.WPF.Services;
using CTime3.Apps.WPF.Views.Home;
using CTime3.Apps.WPF.Views.Login;
using CTime3.Apps.WPF.Views.Settings;
using CTime3.Apps.WPF.Views.Shell;
using CTime3.Core;
using CTime3.Core.Services.Alerts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Contracts;
using Wpf.Ui.Services;

namespace CTime3.Apps.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly IHost s_host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(AppContext.BaseDirectory); })
            .ConfigureServices(services =>
            {
                // App Host
                services.AddHostedService<ShellShellWindowHostedService>();

                // Services
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Shell window with navigation
                services.AddTransient<ShellWindow>();
                services.AddTransient<ShellWindowViewModel>();

                // Views and ViewModels
                services.AddTransient<LoginPage>();
                services.AddTransient<LoginPageViewModel>();

                services.AddTransient<SettingsPage>();
                services.AddTransient<SettingsPageViewModel>();

                services.AddTransient<HomePage>();
                services.AddTransient<HomePageViewModel>();

                // CTime Services
                services.AddCTimeServices(o =>
                {
                    o.CompanyName = "haefele";
                    o.AppName = "c-Time Fluent";
                });

                // Replace CTime Services
                services.Replace(ServiceDescriptor.Singleton<IAlertService, AlertService>());
            }).Build();

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await s_host.StartAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await s_host.StopAsync().ConfigureAwait(false);

            s_host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}

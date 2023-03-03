using System.Windows;
using CTime3.Apps.WPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Mvvm.Contracts;

namespace CTime3.Apps.WPF
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ShowMainWindowHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public ShowMainWindowHostedService(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var navigationWindow = this._serviceProvider.GetRequiredService<MainWindow>();
            navigationWindow.Show();

            navigationWindow.Initialize();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

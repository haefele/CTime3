using System.Windows;
using CommunityToolkit.Diagnostics;
using CTime3.Apps.WPF.Views.Shell;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CTime3.Apps.WPF
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ShellShellWindowHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public ShellShellWindowHostedService(IServiceProvider serviceProvider)
        {
            Guard.IsNotNull(serviceProvider);

            this._serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var navigationWindow = this._serviceProvider.GetRequiredService<ShellWindow>();
            navigationWindow.Show();

            // WORKAROUND: First need to Show the window, so Initialize and initial navigation works correctly
            await navigationWindow.ViewModel.Initialize();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

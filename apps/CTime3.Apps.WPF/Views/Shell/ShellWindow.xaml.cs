using System.Windows;
using CommunityToolkit.Diagnostics;
using CTime3.Apps.WPF.Services;
using CTime3.Apps.WPF.Views.Login;
using CTime3.Apps.WPF.Views.Shell;
using CTime3.Core.Services.Alerts;
using Wpf.Ui.Contracts;

namespace CTime3.Apps.WPF.Views.Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellWindow
    {
        public ShellWindowViewModel ViewModel => (ShellWindowViewModel)this.DataContext;

        public ShellWindow(ShellWindowViewModel viewModel, INavigationService navigationService, IPageService pageService, IAlertService alertService)
        {
            Guard.IsNotNull(viewModel);
            Guard.IsNotNull(navigationService);
            Guard.IsNotNull(pageService);
            Guard.IsNotNull(alertService);

            this.DataContext = viewModel;

            this.InitializeComponent();

            navigationService.SetPageService(pageService);
            navigationService.SetNavigationControl(this.RootNavigation);

            ((AlertService)alertService).SetSnackbar(this.Snackbar);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }
    }
}

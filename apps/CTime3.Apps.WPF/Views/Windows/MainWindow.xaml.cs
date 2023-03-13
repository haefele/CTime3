﻿using System.Windows;
using CommunityToolkit.Diagnostics;
using CTime3.Apps.WPF.Services;
using CTime3.Apps.WPF.Views.Login;
using CTime3.Core.Services.Alerts;
using Wpf.Ui.Contracts;

namespace CTime3.Apps.WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService, IPageService pageService, IAlertService alertService)
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

        public void Initialize()
        {
            this.RootNavigation.Navigate(typeof(LoginPage));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }
    }
}

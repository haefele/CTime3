using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CTime3.Apps.WPF.Views.Home;
using CTime3.Apps.WPF.Views.Login;
using CTime3.Apps.WPF.Views.Settings;
using CTime3.Core;
using CTime3.Core.Services.Configurations;
using CTime3.Core.ViewModels.Login;
using CTime3.Core.ViewModels.Settings;
using Microsoft.Extensions.Options;
using Wpf.Ui.Common;
using Wpf.Ui.Contracts;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;

namespace CTime3.Apps.WPF.Views.Shell
{
    public partial class ShellWindowViewModel : ObservableObject, IRecipient<LoggedInEvent>, IRecipient<LoggedOutEvent>
    {
        private readonly IOptions<CTimeApplicationOptions> _ctimeApplicationOptions;
        private readonly INavigationService _navigationService;
        private readonly IConfigurationService _configurationService;
        private readonly IMessenger _messenger;

        private readonly List<NavigationViewItem> _loginItems;
        private readonly List<NavigationViewItem> _userItems;

        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<NavigationViewItem> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<NavigationViewItem> _footerNavigationItems = new();

        public ShellWindowViewModel(IOptions<CTimeApplicationOptions> ctimeApplicationOptions, INavigationService navigationService, IConfigurationService configurationService, IMessenger messenger)
        {
            Guard.IsNotNull(ctimeApplicationOptions);
            Guard.IsNotNull(navigationService);
            Guard.IsNotNull(configurationService);
            Guard.IsNotNull(messenger);

            this._ctimeApplicationOptions = ctimeApplicationOptions;
            this._navigationService = navigationService;
            this._configurationService = configurationService;
            this._messenger = messenger;

            this.ApplicationTitle = ctimeApplicationOptions.Value.AppName;

            // WORKAROUND: Updating the NavigationItems and FooterNavigationItems later does not update the UI.
            //             But having all items there, and controlling their visibility does work.
            this._loginItems = new List<NavigationViewItem>
            {
                new NavigationViewItem
                {
                    Content = "Login",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(LoginPage),
                    Visibility = Visibility.Collapsed
                }
            };
            this._userItems = new List<NavigationViewItem>
            {
                new NavigationViewItem
                {
                    Content = "Home",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(HomePage),
                    Visibility = Visibility.Collapsed,
                }
            };
            this.NavigationItems = new ObservableCollection<NavigationViewItem>(this._loginItems.Union(this._userItems));

            this.FooterNavigationItems = new ObservableCollection<NavigationViewItem>
            {
                new NavigationViewItem
                {
                    Content = "Settings",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(SettingsPage),
                }
            };

            messenger.RegisterAll(this);
        }

        public async Task Initialize()
        {
            if (this._configurationService.Config.CurrentUser is null)
            {
                this.SetupShellForLogin();
            }
            else
            {
                this.SetupShellForUser();
            }

            await Task.CompletedTask;
        }

        private void SetupShellForLogin()
        {
            foreach (var item in this.NavigationItems)
            {
                item.Visibility = Visibility.Collapsed;
            }
            foreach (var item in this._loginItems)
            {
                item.Visibility = Visibility.Visible;
            }

            this._navigationService.Navigate(typeof(LoginPage));
        }

        private void SetupShellForUser()
        {
            foreach (var item in this.NavigationItems)
            {
                item.Visibility = Visibility.Collapsed;
            }
            foreach (var item in this._userItems)
            {
                item.Visibility = Visibility.Visible;
            }

            this._navigationService.Navigate(typeof(HomePage));
        }

        public void Receive(LoggedInEvent message)
        {
            this.SetupShellForUser();
        }

        public void Receive(LoggedOutEvent message)
        {
            this.SetupShellForLogin();
        }
    }
}

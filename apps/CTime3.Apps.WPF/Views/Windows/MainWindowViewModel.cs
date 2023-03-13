using System.Collections.ObjectModel;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CTime3.Apps.WPF.Views.Login;
using CTime3.Apps.WPF.Views.Settings;
using CTime3.Core;
using Microsoft.Extensions.Options;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Navigation;

namespace CTime3.Apps.WPF.Views.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<INavigationViewItem> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<INavigationViewItem> _footerNavigationItems = new();

        public MainWindowViewModel(IOptions<CTimeApplicationOptions> ctimeApplicationOptions)
        {
            Guard.IsNotNull(ctimeApplicationOptions);

            this.ApplicationTitle = ctimeApplicationOptions.Value.AppName;

            this.NavigationItems.Add(new NavigationViewItem
            {
                Content = "Login",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(LoginPage),
            });

            this.FooterNavigationItems.Add(new NavigationViewItem
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(SettingsPage),
            });
        }
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CTime3.Apps.WPF.Views.Login;
using CTime3.Core;
using Microsoft.Extensions.Options;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;

namespace CTime3.Apps.WPF.Views.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = string.Empty;

        [ObservableProperty]
        private ObservableCollection<INavigationControl> _navigationItems = new();

        public MainWindowViewModel(IOptions<CTimeApplicationOptions> ctimeApplicationOptions)
        {
            Guard.IsNotNull(ctimeApplicationOptions);

            this.ApplicationTitle = ctimeApplicationOptions.Value.AppName;

            this.NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Login",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(LoginPage),
                },
            };
        }
    }
}

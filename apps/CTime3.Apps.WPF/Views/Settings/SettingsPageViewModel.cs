using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CTime3.Core;
using CTime3.Core.ViewModels.Settings;
using Microsoft.Extensions.Options;
using Wpf.Ui.Appearance;

namespace CTime3.Apps.WPF.Views.Settings
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ThemeType _currentTheme = ThemeType.Unknown;

        [ObservableProperty]
        private SettingsViewModel _core;

        public SettingsPageViewModel(IOptions<CTimeApplicationOptions> ctimeApplicationOptions, SettingsViewModel coreViewModel)
        {
            Guard.IsNotNull(ctimeApplicationOptions);
            Guard.IsNotNull(coreViewModel);

            this._core = coreViewModel;

            this.CurrentTheme = Theme.GetAppTheme();
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            Guard.IsNotNullOrWhiteSpace(parameter);

            var newTheme = parameter switch
            {
                "theme_light" => ThemeType.Light,
                _ => ThemeType.Dark
            };

            if (this.CurrentTheme == newTheme)
                return;

            Theme.Apply(newTheme);
            this.CurrentTheme = newTheme;
        }
    }
}

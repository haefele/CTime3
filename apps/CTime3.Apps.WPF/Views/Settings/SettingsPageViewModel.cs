﻿using System.Reflection;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CTime3.Core;
using Microsoft.Extensions.Options;
using Wpf.Ui.Appearance;

namespace CTime3.Apps.WPF.Views.Settings
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _appVersion = string.Empty;

        [ObservableProperty]
        private ThemeType _currentTheme = ThemeType.Unknown;

        public SettingsPageViewModel(IOptions<CTimeApplicationOptions> ctimeApplicationOptions)
        {
            Guard.IsNotNull(ctimeApplicationOptions);

            this.CurrentTheme = Theme.GetAppTheme();
            this.AppVersion = $"{ctimeApplicationOptions.Value.AppName} - {GetAssemblyVersion()}";
        }

        private static string GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
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
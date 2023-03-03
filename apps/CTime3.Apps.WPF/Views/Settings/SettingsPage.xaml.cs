using CTime3.Apps.WPF.Views.Settings;
using Wpf.Ui.Common.Interfaces;

namespace CTime3.Apps.WPF.Views.Settings
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}

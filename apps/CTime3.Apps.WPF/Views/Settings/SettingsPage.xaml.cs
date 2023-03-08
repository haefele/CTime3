using CommunityToolkit.Diagnostics;

namespace CTime3.Apps.WPF.Views.Settings
{
    public partial class SettingsPage
    {
        public SettingsPage(SettingsPageViewModel pageViewModel)
        {
            Guard.IsNotNull(pageViewModel);

            this.DataContext = pageViewModel;

            this.InitializeComponent();
        }
    }
}

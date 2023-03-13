using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Diagnostics;

namespace CTime3.Apps.WPF.Views.Login;

public partial class LoginPage
{
    public LoginPage(LoginPageViewModel viewModel)
    {
        Guard.IsNotNull(viewModel);

        this.DataContext = viewModel;

        this.InitializeComponent();
    }

    public LoginPageViewModel ViewModel => (LoginPageViewModel)this.DataContext;

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        // WORKAROUND: For some reason the password binding doesn't work
        this.ViewModel.Core.Password = ((Wpf.Ui.Controls.PasswordBox)sender).Password;
    }
}


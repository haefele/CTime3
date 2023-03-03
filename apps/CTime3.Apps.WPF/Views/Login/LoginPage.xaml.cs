using System.Windows.Controls;
using CommunityToolkit.Diagnostics;
using Wpf.Ui.Common.Interfaces;

namespace CTime3.Apps.WPF.Views.Login;

public partial class LoginPage
{
    public LoginPage(LoginPageViewModel viewModel)
    {
        Guard.IsNotNull(viewModel);
        this.DataContext = this.ViewModel = viewModel;

        this.InitializeComponent();
    }

    public LoginPageViewModel ViewModel { get; }
}


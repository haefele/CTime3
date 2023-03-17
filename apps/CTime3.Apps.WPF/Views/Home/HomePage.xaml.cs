using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using CTime3.Core.Services.Configurations;
using CTime3.Core.ViewModels.Login;
using CTime3.Core.ViewModels.Settings;

namespace CTime3.Apps.WPF.Views.Home;

public partial class HomePage
{
    public HomePage(HomePageViewModel viewModel)
    {
        Guard.IsNotNull(viewModel);

        this.DataContext = viewModel;

        this.InitializeComponent();
    }
}


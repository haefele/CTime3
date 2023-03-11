using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using CTime3.Core.Services.Configurations;
using CTime3.Core.ViewModels.Login;

namespace CTime3.Apps.WPF.Views.Home;

public partial class HomePage
{
    private readonly IMessenger _messenger;
    private readonly IConfigurationService _configurationService;

    public HomePage(HomePageViewModel viewModel, IMessenger messenger, IConfigurationService configurationService)
    {
        Guard.IsNotNull(viewModel);
        Guard.IsNotNull(messenger);
        Guard.IsNotNull(configurationService);

        this._messenger = messenger;
        this._configurationService = configurationService;

        this.DataContext = viewModel;

        this.InitializeComponent();
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        await this._configurationService.Modify(config => config with { CurrentUser = null });
        this._messenger.Send(new LoggedOutEvent());
    }
}


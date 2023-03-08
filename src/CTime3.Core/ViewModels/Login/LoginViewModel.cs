using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CTime3.Core.Services.Alerts;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;

namespace CTime3.Core.ViewModels.Login;

public partial class LoginViewModel : ObservableObject
{
    private readonly ICTimeService _ctimeService;
    private readonly IConfigurationService _configurationService;
    private readonly IMessenger _messenger;
    private readonly IAlertService _alertService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _loginName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    public LoginViewModel(ICTimeService ctimeService, IConfigurationService configurationService, IMessenger messenger, IAlertService alertService)
    {
        Guard.IsNotNull(ctimeService);
        Guard.IsNotNull(configurationService);
        Guard.IsNotNull(messenger);
        Guard.IsNotNull(alertService);

        this._ctimeService = ctimeService;
        this._configurationService = configurationService;
        this._messenger = messenger;
        this._alertService = alertService;
    }

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        var user = await this._ctimeService.Login(this.LoginName, this.Password);
        if (user is not null)
        {
            await this._configurationService.Modify(config => config with { CurrentUser = CurrentUser.FromUser(user) });

            await this._alertService.Show("Login successful!", "You are now logged in. Have fun using this app.", AlertType.Success);
            this._messenger.Send(new LoggedInEvent());
        }
        else
        {
            await this._alertService.Show("Login failed!", "Login failed. Please make sure you entered your username and password correctly.", AlertType.Error);
        }
    }
    private bool CanLogin() => string.IsNullOrWhiteSpace(this.LoginName) is false &&
                               string.IsNullOrWhiteSpace(this.Password) is false;
}

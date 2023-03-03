using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CTime3.Core.Services.CTime;

namespace CTime3.Core.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ICTimeService _ctimeService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _loginName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    public LoginViewModel(ICTimeService ctimeService)
    {
        Guard.IsNotNull(ctimeService);

        this._ctimeService = ctimeService;
    }

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        var user = await this._ctimeService.Login(this.LoginName, this.Password);
        if (user is not null)
        {
            // Success
        }
        else
        {
            // Failure
        }
    }
    private bool CanLogin() => string.IsNullOrWhiteSpace(this.LoginName) is false &&
                               string.IsNullOrWhiteSpace(this.Password) is false;
}

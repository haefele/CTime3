using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CTime3.Core.Services.Configurations;
using Microsoft.Extensions.Options;

namespace CTime3.Core.ViewModels.Settings;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IConfigurationService _configurationService;
    private readonly IMessenger _messenger;
    private readonly IOptions<CTimeApplicationOptions> _ctimeApplicationOptions;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LogoutCommand))]
    private bool _isLoggedIn;

    [ObservableProperty]
    private string _appName;

    [ObservableProperty]
    private string? _companyName;

    [ObservableProperty]
    private string? _versionNumber;

    public SettingsViewModel(IConfigurationService configurationService, IMessenger messenger, IOptions<CTimeApplicationOptions> ctimeApplicationOptions)
    {
        Guard.IsNotNull(configurationService);
        Guard.IsNotNull(messenger);
        Guard.IsNotNull(ctimeApplicationOptions);

        this._configurationService = configurationService;
        this._messenger = messenger;
        this._ctimeApplicationOptions = ctimeApplicationOptions;

        this._isLoggedIn = this._configurationService.Config.CurrentUser is not null;
        this._appName = this._ctimeApplicationOptions.Value.AppName;
        this._companyName = this._ctimeApplicationOptions.Value.CompanyName;
        this._versionNumber = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }

    [RelayCommand(CanExecute = nameof(CanLogout))]
    private async Task Logout()
    {
        await this._configurationService.Modify(config => config with { CurrentUser = null });
        this._messenger.Send(new LoggedOutEvent());
    }
    private bool CanLogout() => this.IsLoggedIn;
}

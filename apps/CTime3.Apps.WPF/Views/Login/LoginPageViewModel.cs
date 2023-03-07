using CommunityToolkit.Mvvm.ComponentModel;
using CTime3.Core.ViewModels.Login;

namespace CTime3.Apps.WPF.Views.Login;

public class LoginPageViewModel : ObservableObject
{
    public LoginPageViewModel(LoginViewModel coreViewModel)
    {
        this.Core = coreViewModel;
    }

    public LoginViewModel Core { get; }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CTime3.Core.ViewModels;

namespace CTime3.Apps.WPF.Views.Login;

public class LoginPageViewModel : ObservableObject
{
    public LoginPageViewModel(LoginViewModel innerViewModel)
    {
        this.Inner = innerViewModel;
    }

    public LoginViewModel Inner { get; }
}

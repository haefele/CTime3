using CTime3.Core.Services.Alerts;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace CTime3.Apps.WPF.Services;

public class AlertService : IAlertService
{
    private Snackbar? _snackbar;

    public void SetSnackbar(Snackbar snackbar)
    {
        this._snackbar = snackbar;
    }

    public async Task Show(string title, string message, AlertType type)
    {
        if (this._snackbar is null)
            return;

        var symbol = type switch
        {
            AlertType.Success => SymbolRegular.Check20,
            AlertType.Error => SymbolRegular.ErrorCircle20,
            _ => SymbolRegular.Bug20,
        };
        var appearance = type switch
        {
            AlertType.Success => ControlAppearance.Success,
            AlertType.Error => ControlAppearance.Danger,
            _ => ControlAppearance.Caution,
        };

#pragma warning disable CA1849 // We don't want to wait until the snackbar is closed again
        this._snackbar.Show(title, message, symbol, appearance);
#pragma warning restore CA1849

        await Task.CompletedTask;
    }
}

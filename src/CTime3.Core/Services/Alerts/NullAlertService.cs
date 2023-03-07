namespace CTime3.Core.Services.Alerts;

public class NullAlertService : IAlertService
{
    public Task Show(string title, string message, AlertType type)
    {
        return Task.CompletedTask;
    }
}

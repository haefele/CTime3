namespace CTime3.Core.Services.Alerts;

public interface IAlertService
{
    Task Show(string title, string message, AlertType type);
}

public enum AlertType
{
    Success,
    Error,
}

namespace CTime3.Core.Services.CTime;

public interface ICTimeService
{
    Task<User?> Login(string emailAddress, string password);
    Task<List<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end);
    Task SaveTimer(string employeeGuid, string? rfidKey, DateTime time, string companyId, TimeState state, bool withGeolocation);
    Task<Time?> GetCurrentTime(string employeeGuid);
    Task<List<AttendingUser>> GetAttendingUsers(string companyId, byte[]? defaultImage);
}

public static class CTimeServiceExtensions
{
    public static async Task<bool> CheckConnection(this ICTimeService self)
    {
        try
        {
            await self.Login(string.Empty, string.Empty);
            return true;
        }
        catch (CTimeException)
        {
            return false;
        }
    }

    public static async Task SaveTimer(this ICTimeService self, User user, DateTime time, TimeState state)
    {
        await self.SaveTimer(user.Id, string.Empty, time, user.CompanyId, state, user.SupportsGeoLocation);
    }

    public static async Task<bool> IsCurrentlyCheckedIn(this ICTimeService self, string employeeGuid)
    {
        var currentTime = await self.GetCurrentTime(employeeGuid);
        return currentTime is not null && currentTime.State.IsEntered();
    }
}
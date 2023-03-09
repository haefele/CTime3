using CTime3.Core.Services.CTime;

namespace CTime3.Core.Services.Configurations;

public record Configuration(
    CurrentUser? CurrentUser,
    TimeSpan BreakTimeBegin,
    TimeSpan BreakTimeEnd,
    TimeSpan WorkDayBreak,
    TimeSpan WorkDayHours)
{
    public static Configuration GetDefault()
    {
        return new(
            CurrentUser: null,
            BreakTimeBegin: TimeSpan.FromHours(11),
            BreakTimeEnd: TimeSpan.FromHours(14.5),
            WorkDayBreak: TimeSpan.FromHours(1),
            WorkDayHours: TimeSpan.FromHours(8));
    }
}

public record CurrentUser(
    string Id,
    string CompanyId,
    string? CompanyName,
    string? FirstName,
    string? Name,
    string? LoginName)
{
    public static CurrentUser FromUser(User user)
    {
        Guard.IsNotNull(user);

        return new(user.Id, user.CompanyId, user.CompanyName, user.FirstName, user.Name, user.LoginName);
    }
}

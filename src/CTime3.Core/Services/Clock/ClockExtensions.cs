namespace CTime3.Core.Services.Clock;

public static class ClockExtensions
{
    public static DateTime Today(this IClock self)
    {
        Guard.IsNotNull(self);

        return self.Now().Date;
    }
}

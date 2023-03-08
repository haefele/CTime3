namespace CTime3.Core.Services.Clock;

public interface IClock
{
    DateTimeOffset Now();
    DateTimeOffset NowUtc();
}

using System;
using CTime3.Core.Services.CTime;

namespace CTime3.Core.Services.Statistics
{
    public interface IStatisticsService
    {
        CurrentTime CalculateCurrentTime(Time? currentTime);
    }

    public record CurrentTime(TimeSpan WorkTime, TimeSpan? OverTime, CurrentBreak? CurrentBreak, bool IsStillRunning);
    public record CurrentBreak(TimeSpan BreakTime, DateTime PreferredBreakTimeEnd);
}
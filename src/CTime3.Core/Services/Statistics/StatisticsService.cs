using System;
using CommunityToolkit.Diagnostics;
using CTime3.Core.Services.Clock;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;

namespace CTime3.Core.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IClock _clock;
        private readonly IConfigurationService _configurationService;

        public StatisticsService(IClock clock, IConfigurationService configurationService)
        {
            Guard.IsNotNull(clock);
            Guard.IsNotNull(configurationService);

            this._clock = clock;
            this._configurationService = configurationService;
        }

        public CurrentTime CalculateCurrentTime(Time? currentTime)
        {
            if (currentTime is null)
                return new CurrentTime(TimeSpan.Zero, null, null, false);

            var now = this._clock.Now().DateTime;

            //Only take the timeToday if the time is either
            // - from today
            // - or from yesterday, but still checked-in
            var timeToday = currentTime.Day == now.Date || currentTime.State.IsEntered()
                ? currentTime.Hours
                : TimeSpan.Zero;

            if (currentTime.State.IsEntered())
                timeToday += now - currentTime.ClockInTime!.Value;

            CurrentBreak? breakTime = null;

            if (currentTime.Day == now.Date &&
                currentTime.State.IsLeft() &&
                currentTime.ClockOutTime.HasValue &&
                currentTime.ClockOutTime.Value.TimeOfDay >= this._configurationService.Config.BreakTimeBegin &&
                currentTime.ClockOutTime.Value.TimeOfDay <= this._configurationService.Config.BreakTimeEnd &&
                now.TimeOfDay >= this._configurationService.Config.BreakTimeBegin &&
                now.TimeOfDay <= this._configurationService.Config.BreakTimeEnd)
            {
                breakTime = new CurrentBreak(now - currentTime.ClockOutTime.Value, currentTime.ClockOutTime.Value.Add(this._configurationService.Config.WorkDayBreak));
            }

            TimeSpan? overtime = null;

            if (timeToday - this._configurationService.Config.WorkDayHours > TimeSpan.FromSeconds(1))
            {
                overtime = timeToday - this._configurationService.Config.WorkDayHours;
                timeToday = this._configurationService.Config.WorkDayHours;
            }

            return new CurrentTime(timeToday, overtime, breakTime, currentTime.State.IsEntered() || breakTime is not null);
        }
    }
}

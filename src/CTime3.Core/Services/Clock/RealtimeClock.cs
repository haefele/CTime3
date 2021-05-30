using System;

namespace CTime3.Core.Services.Clock
{
    public class RealtimeClock : IClock
    {
        public DateTimeOffset Now()
        {
            return DateTimeOffset.Now;
        }

        public DateTimeOffset NowUtc()
        {
            return DateTimeOffset.UtcNow;
        }
    }
}

using System;
using System.Collections.Generic;

namespace CTime3.Core.Services.Analytics
{
    public interface IAnalyticsService
    {
        void TrackEvent(string eventName, IDictionary<string, string>? properties = null);
        void TrackException(Exception exception);
    }
}

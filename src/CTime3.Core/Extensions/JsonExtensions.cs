using System;
using Newtonsoft.Json.Linq;

namespace CTime3.Core.Extensions
{
    public static class JsonExtensions
    {
        public static TimeSpan ValueAsTimeSpan(this JObject self, string name)
        {
            var time = self.Value<string>(name);
            
            if (string.IsNullOrWhiteSpace(time))
                return TimeSpan.Zero;
            
            string[] parts = time.Split(':');

            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);

            return new TimeSpan(hours, minutes, 0);
        }
    }
}
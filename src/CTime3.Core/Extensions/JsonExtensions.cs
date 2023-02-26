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

            var parts = time.Split(':');

            var hours = int.Parse(parts[0]);
            var minutes = int.Parse(parts[1]);

            return new TimeSpan(hours, minutes, 0);
        }

        public static byte[] ValueAsBase64Array(this JObject self, string name)
        {
            var base64 = self.Value<string>(name);

            if (string.IsNullOrWhiteSpace(base64))
                return Array.Empty<byte>();

            return Convert.FromBase64String(base64);
        }
    }
}

using System.Globalization;
using Newtonsoft.Json.Linq;

namespace CTime3.Core.Extensions;

public static class JsonExtensions
{
    public static TimeSpan ValueAsTimeSpan(this JObject self, string name)
    {
        Guard.IsNotNull(self);

        var time = self.Value<string>(name);

        if (string.IsNullOrWhiteSpace(time))
            return TimeSpan.Zero;

        var parts = time.Split(':');

        var hours = int.Parse(parts[0], CultureInfo.InvariantCulture);
        var minutes = int.Parse(parts[1], CultureInfo.InvariantCulture);

        return new TimeSpan(hours, minutes, 0);
    }
}

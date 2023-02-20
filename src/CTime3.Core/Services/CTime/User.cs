using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTime3.Core.Services.CTime
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public byte[]? ImageAsPng { get; set; }
        public bool SupportsGeoLocation { get; set; }
        public byte[]? CompanyImageAsPng { get; set; }
    }

    public class Time
    {
        public DateTime Day { get; set; }
        public TimeSpan Hours { get; set; }
        public TimeState? State { get; set; }
        public string? StateDescription { get; set; }
        public DateTime? ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
    }

    [Flags]
    public enum TimeState
    {
        Entered = 1,
        Left = 2,
        ShortBreak = 4,
        Trip = 8,
        HomeOffice = 16
    }

    public static class TimeStateExtensions
    {
        public static bool IsEntered(this TimeState self)
        {
            return self.HasFlag(TimeState.Entered);
        }
        public static bool IsEntered(this TimeState? self)
        {
            return self is not null && self.Value.IsEntered();
        }

        public static bool IsLeft(this TimeState self)
        {
            return self.HasFlag(TimeState.Left);
        }

        public static bool IsLeft(this TimeState? self)
        {
            return self is not null && self.Value.IsLeft();
        }

        public static bool IsTrip(this TimeState self)
        {
            return self.HasFlag(TimeState.Trip);
        }

        public static bool IsTrip(this TimeState? self)
        {
            return self is not null && self.Value.IsTrip();
        }

        public static bool IsHomeOffice(this TimeState self)
        {
            return self.HasFlag(TimeState.HomeOffice);
        }

        public static bool IsHomeOffice(this TimeState? self)
        {
            return self is not null && self.Value.IsHomeOffice();
        }
    }

    public class AttendingUser
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public AttendanceState AttendanceState { get; set; } = null!;
        public byte[] ImageAsPng { get; set; } = Array.Empty<byte>();
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string[]? Departments { get; set; }
    }

    public class AttendanceState
    {
        public bool IsAttending { get; set; }
        public string? Name { get; set; }
        public Color? Color { get; set; }
    }

    public class Color
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static Color Transparent { get; } = new Color { A = 0, R = 0, G = 0, B = 0 };

        internal static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color
            {
                A = a,
                R = r,
                G = g,
                B = b
            };
        }
    }
}

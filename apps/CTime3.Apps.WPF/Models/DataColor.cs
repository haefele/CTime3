using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CTime3.Apps.WPF.Models
{
    public struct DataColor : IEquatable<DataColor>
    {
        public Brush Color { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DataColor color &&
                   EqualityComparer<Brush>.Default.Equals(Color, color.Color);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color);
        }

        public static bool operator ==(DataColor left, DataColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DataColor left, DataColor right)
        {
            return !(left == right);
        }

        public bool Equals(DataColor other)
        {
            return Equals((object)other);
        }
    }
}

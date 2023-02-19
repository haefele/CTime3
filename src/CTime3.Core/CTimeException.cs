using System;
using CommunityToolkit.Diagnostics;

namespace CTime3.Core
{
    public class CTimeException : Exception
    {
        public CTimeException(string message)
            : base(message)
        {
            Guard.IsNotNull(message, nameof(message));
        }

        public CTimeException(string message, Exception inner)
            : base(message, inner)
        {
            Guard.IsNotNull(message, nameof(message));
            Guard.IsNotNull(inner, nameof(inner));
        }
    }
}

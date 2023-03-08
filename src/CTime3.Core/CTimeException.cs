namespace CTime3.Core;

public class CTimeException : Exception
{
    public CTimeException()
    {
    }
    public CTimeException(string message)
        : base(message)
    {
        Guard.IsNotNull(message);
    }

    public CTimeException(string message, Exception inner)
        : base(message, inner)
    {
        Guard.IsNotNull(message);
        Guard.IsNotNull(inner);
    }
}

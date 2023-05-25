using System.Reflection;
using System.Text;

namespace CTime3.Core.Extensions;

public static class ExceptionExtensions
{
    public static string GetFullMessage(this Exception self)
    {
        Guard.IsNotNull(self);

        var sb = new StringBuilder();
        foreach (var ex in self.GetUserRelevantExceptions())
        {
            sb.AppendLine(ex.Message);
        }

        return sb.ToString();
    }

    public static IEnumerable<Exception> GetUserRelevantExceptions(this Exception self)
    {
        Guard.IsNotNull(self);

        return self.GetAllExceptions()
            .Where(f => f is not AggregateException)
            .Where(f => f is not TargetInvocationException);
    }

    public static IEnumerable<Exception> GetAllExceptions(this Exception self)
    {
        Guard.IsNotNull(self);

        var stack = new Stack<Exception>();
        stack.Push(self);

        while (stack.TryPop(out var current))
        {
            yield return current;

            switch (current)
            {
                case AggregateException { InnerExceptions: not null } aggEx:
                    {
                        foreach (var inner in aggEx.InnerExceptions)
                        {
                            stack.Push(inner);
                        }

                        break;
                    }
                case { InnerException: not null }:
                    {
                        stack.Push(current.InnerException);
                    }
                    break;
            }
        }
    }
}

using System.Globalization;

namespace CTime3.Core.Extensions;

public static class StringExtensions
{
    public static string MakeFirstCharacterUpperCase(this string self)
    {
        if (string.IsNullOrWhiteSpace(self))
            return self;

        var firstCharacter = self.Substring(0, 1);
        var rest = self.Substring(1);

        return firstCharacter.ToUpper(CultureInfo.CurrentUICulture) + rest;
    }
}

using System.Globalization;
using System.Text;

namespace TempusHub.Core.Utilities;

public static class StringExtensions
{
    public static string ToStandardCasing(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }
        var inputParts = input.Split(' ', '_', '-');

        var stringBuilder = new StringBuilder();
        foreach (var part in inputParts)
        {
            stringBuilder.Append(char.ToUpper(part[0], CultureInfo.InvariantCulture) + part.Substring(1).ToLower(CultureInfo.InvariantCulture) + " ");
        }

        return stringBuilder.ToString().TrimEnd(' ');
    }
}
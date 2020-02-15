using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempusHubBlazor.Utilities
{
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
                stringBuilder.Append(char.ToUpper(part[0]) + part.Substring(1).ToLower() + " ");
            }

            return stringBuilder.ToString().TrimEnd(' ');
        }
    }
}

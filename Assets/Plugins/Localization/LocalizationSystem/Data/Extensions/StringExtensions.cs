using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;


namespace LocalizationSystem.Data.Extensions
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string str)
        {
            var camelCaseString = str.ToCamelCase();
            return camelCaseString.FirstCharacterToUpper();
        }

        public static string ToCamelCase(this string str)
        {
            var x = str.StripPunctuation();

            if (x.Length == 0) return "null";
            x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
                m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
            return char.ToLower(x[0]) + x.Substring(1);
        }

        public static string StripPunctuation(this string s)
        {
            var sb = new StringBuilder();
            foreach (var c in s.Where(c => !char.IsPunctuation(c) && c != '_' && c != ' '))
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
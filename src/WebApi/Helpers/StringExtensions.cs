using System.Text.RegularExpressions;

namespace WebApi.Helpers
{
    public static class StringExtensions
    {
        public static string RemoveAllNonPrintableCharacters(this string target)
        {
            return Regex.Replace(target, @"\p{C}+", string.Empty);
        }
    }
}

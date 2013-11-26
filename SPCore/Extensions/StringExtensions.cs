using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.SharePoint.Utilities;

namespace SPCore.Extensions
{
    public static class StringExtensions
    {
        public static string Localize(this string source)
        {
            return Localize(source, (uint)Thread.CurrentThread.CurrentCulture.LCID);
        }

        public static string Localize(this string source, uint language)
        {
            var pattern = string.Format("^{0}$",
            Regex.Escape("$Resources:FILE,KEY;").
            Replace("FILE", @"\w+").
            Replace("KEY", ".+"));

            if (!Regex.IsMatch(source, pattern)) return source;
            var parts = source.Split(new[] { ':', ',' }, 3);
            var file = parts[1];
            var key = parts[2];
            return SPUtility.GetLocalizedString(string.Format("$Resources:{0}", key), file, language);
        }
    }
}

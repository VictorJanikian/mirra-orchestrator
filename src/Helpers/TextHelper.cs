using System.Text.RegularExpressions;

namespace Mirra_Orchestrator.Helpers
{
    public static class TextHelper
    {
        public static string RemoveHtmlTags(string text)
        {

            if (string.IsNullOrEmpty(text))
                return text;

            return Regex.Replace(text, "<.*?>", string.Empty, RegexOptions.Singleline);
        }
    }
}

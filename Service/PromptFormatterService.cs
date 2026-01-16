using Microsoft.IdentityModel.Tokens;
using Mirra_Orchestrator.Helpers;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;
using System.Text.RegularExpressions;
using static Mirra_Orchestrator.Helpers.ListHelper;

namespace Mirra_Orchestrator.Service
{
    public class PromptFormatterService : IPromptFormatterService
    {
        public async Task<string> ReplacePromptVariables(string prompt, Parameters parameters, List<Content> lastContents)
        {
            var replacements = new Dictionary<string, string> {
                { "ThemeTitle",parameters.ThemeTitle },
                { "Description", NotInformedIfEmpty(parameters.Description) },
                { "Keywords", NotInformedIfEmpty(parameters.Keywords) },
                { "TargetAudience", NotInformedIfEmpty(parameters.TargetAudience) },
                { "Style", NotInformedIfEmpty(parameters.Style) },
                { "Goal", NotInformedIfEmpty(parameters.Goal) },
                { "ApproximatedSize", NotInformedIfEmpty(parameters.ApproximatedSize) },
                { "AdditionalInfo", NotInformedIfEmpty(parameters.AdditionalInfo) },
                { "Categories", NotInformedIfEmpty(parameters.Categories) },
                { "Tags", NotInformedIfEmpty(parameters.Tags) },
                { "SEOAdditionalInformation", NotInformedIfEmpty(parameters.SEOAdditionalInformation) },
                { "Language", NotInformedIfEmpty(parameters.Language) },
                { "CTA", NotInformedIfEmpty(parameters.CTA) },
                { "SearchIntent", NotInformedIfEmpty(parameters.SearchIntent) },
                { "LastContents", NotInformedIfEmpty(getLastContents(lastContents)) },
            };


            string result = Regex.Replace(prompt, @"\{\{\s*(.*?)\s*\}\}", match =>
            {
                var key = match.Groups[1].Value;
                return replacements.TryGetValue(key, out var value)
                    ? value
                    : match.Value;
            });

            return result;
        }

        private string NotInformedIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "(not informed)"
                : value;
        }



        public async Task<string> ReplaceTextInsidePrompt(string prompt, string text)
        {
            var replacements = new Dictionary<string, string> {
                { "text", text }
            };

            string result = Regex.Replace(prompt, @"\{\{\s*(.*?)\s*\}\}", match =>
            {
                var key = match.Groups[1].Value;
                return replacements.TryGetValue(key, out var value)
                    ? value
                    : match.Value;
            });

            return result;
        }

        private string getLastContents(List<Content> lastPosts)
        {
            if (lastPosts.IsNullOrEmpty()) return string.Empty;

            var lastPostsString = string.Empty;

            for (int i = 0; i < lastPosts.Count(); i++)
            {
                lastPostsString += i + 1 + ": Título: " + lastPosts[i].ContentTitle + " Resumo: " + lastPosts[i].ContentSummary + ", ";
            }

            lastPostsString = lastPostsString.Substring(0, lastPostsString.Length - 2);
            lastPostsString += ".";
            return lastPostsString;
        }
    }
}

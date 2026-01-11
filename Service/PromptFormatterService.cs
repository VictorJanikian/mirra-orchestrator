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
                { "ThemeTitle", parameters.ThemeTitle },
                { "Keywords", parameters.Keywords },
                { "TargetAudience", parameters.TargetAudience },
                { "Style", parameters.Style },
                { "Goal", parameters.Goal },
                { "ApproximatedSize", parameters.ApproximatedSize },
                { "AdditionalInfo", parameters.AdditionalInfo },
                { "Categories", parameters.Categories },
                { "Tags", parameters.Tags },
                { "SEOAdditionalInformation", parameters.SEOAdditionalInformation },
                { "Language", parameters.Language},
                { "CTA", parameters.CTA},
                { "LastContents", getLastContents(lastContents)},

            };

            var promptWithNoUndefinedParameters = removeOptionalParametersNotPresent(prompt, parameters, lastContents);

            string result = Regex.Replace(promptWithNoUndefinedParameters, @"\{\{\s*(.*?)\s*\}\}", match =>
            {
                var key = match.Groups[1].Value;
                return replacements.TryGetValue(key, out var value)
                    ? value
                    : match.Value;
            });

            return result;
        }

        private string removeOptionalParametersNotPresent(string prompt, Parameters parameters, List<Content> LastContents)
        {
            List<(string Name, object? Value)> emptyOrNullParameters = getEmptyOrNullParameters(parameters);

            if (LastContents.IsNullOrEmpty())
                emptyOrNullParameters.Add(("LastContents", null));

            if (!emptyOrNullParameters.Any())
                return prompt;

            var pattern = @"\{\{\s*(" + string.Join("|", emptyOrNullParameters.Select(p => Regex.Escape(p.Name))) + @")\s*\}\}";

            var sentencesWithNotNullParameters = prompt
                .Split("&!", StringSplitOptions.RemoveEmptyEntries)
                .Select(sentence => sentence.Trim())
                .Where(sentence => !Regex.IsMatch(sentence, pattern))
                .ToList();

            return string.Join(" ", sentencesWithNotNullParameters);
        }

        private static List<(string Name, object? Value)> getEmptyOrNullParameters(Parameters parameters)
        {
            return typeof(Parameters).GetProperties()
                                     .Select(property => (Name: property.Name, Value: property.GetValue(parameters)))
                                     .Where(property => property.Value == null || property.Value.Equals(""))
                                     .ToList();
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

            foreach (var post in lastPosts)
            {
                lastPostsString += "Título: " + post.ContentTitle + " Resumo: " + post.ContentSummary + ", ";
            }

            lastPostsString = lastPostsString.Substring(0, lastPostsString.Length - 2);
            lastPostsString += ".";
            return lastPostsString;
        }
    }
}

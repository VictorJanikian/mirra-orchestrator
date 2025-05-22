using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;
using System.Text.RegularExpressions;

namespace Mirra_Orchestrator.Service
{
    public class PromptFormatterService : IPromptFormatterService
    {
        public async Task<string> ReplacePromptVariables(string prompt, Parameters parameters)
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

            };

            var promptWithNoUndefinedParameters = removeOptionalParametersNotPresent(prompt, parameters);

            string result = Regex.Replace(promptWithNoUndefinedParameters, @"\{\{\s*(.*?)\s*\}\}", match =>
            {
                var key = match.Groups[1].Value;
                return replacements.TryGetValue(key, out var value)
                    ? value
                    : match.Value;
            });

            return result;
        }

        private string removeOptionalParametersNotPresent(string prompt, Parameters parameters)
        {
            List<(string Name, object? Value)> emptyOrNullParameters = getEmptyOrNullParameters(parameters);

            if (!emptyOrNullParameters.Any())
                return prompt;

            var pattern = @"\{\{\s*(" + string.Join("|", emptyOrNullParameters.Select(p => Regex.Escape(p.Name))) + @")\s*\}\}";

            var sentencesWithNotNullParameters = prompt
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(sentence => sentence.Trim())
                .Where(sentence => !Regex.IsMatch(sentence, pattern))
                .ToList();

            return string.Join(". ", sentencesWithNotNullParameters) + (prompt.EndsWith(".") ? "." : "");

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
    }
}

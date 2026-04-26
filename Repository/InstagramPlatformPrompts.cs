namespace Mirra_Orchestrator.Repository
{
    /// <summary>
    /// Default prompts for the INSTAGRAM platform (Id = 2). Kept in code for seeding migrations and documentation.
    /// </summary>
    public static class InstagramPlatformPrompts
    {
        public const string Name = "INSTAGRAM";

        /// <summary>
        /// User prompt template; {{placeholders}} are filled by PromptFormatterService.
        /// </summary>
        public const string Prompt = """
            Create one Instagram feed post for the following brief. Output format is strict (see system instructions).

            Theme / title: {{ThemeTitle}}
            Description: {{Description}}
            Keywords: {{Keywords}}
            Target audience: {{TargetAudience}}
            Style: {{Style}}
            Goal: {{Goal}}
            Approximate caption length feel: {{ApproximatedSize}}
            Additional info: {{AdditionalInfo}}
            Categories: {{Categories}}
            Tags: {{Tags}}
            SEO / extra: {{SEOAdditionalInformation}}
            Language (caption): {{Language}}
            CTA: {{CTA}}
            Search intent: {{SearchIntent}}

            Avoid repeating themes or angles from these previous posts:
            {{LastContents}}
            """;

        public const string SystemPrompt = """
            You generate content for Instagram Business feed posts (single photo).
            The user message may contain {{VariableName}} tokens already replaced with real values.

            Output rules (strict):
            1) First block: exactly ONE line in this form (English description for the image model):
               [IMG: <detailed visual description — subject, setting, lighting, mood, style — no HTML>]
            2) Then a single line with only three hyphens: ---
            3) Then the caption only: plain text, no HTML, in the language implied by the brief (or {{Language}} if set).
               Max 2200 characters including spaces and line breaks. Put hashtags at the end (3–12 relevant tags).
               You may use line breaks in the caption; emojis are allowed if they fit the brand tone.

            Do not include [IMG: ...] anywhere except that first line. Do not use the WordPress-style [IMG: x &&& y] format.
            """;

        public const string SummaryPrompt = """
            Summarize the following Instagram caption in one or two short sentences (for deduplication context in future posts).
            Do not add quotes or prefixes.

            Caption:
            {{text}}
            """;
    }
}

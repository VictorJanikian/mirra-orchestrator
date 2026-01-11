using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IPromptFormatterService
    {
        Task<string> ReplacePromptVariables(string prompt, Parameters parameters, List<Content> lastPosts);

        Task<string> ReplaceTextInsidePrompt(string prompt, string text);

    }
}

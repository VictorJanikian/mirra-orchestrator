using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IContentGenerationService
    {
        Task<WordpressBlogPost> GenerateBlogPost(Parameters parameters, string prompt);
        Task<string> GenerateBlogPostSummary(string originalPost, string summaryPrompt);

    }
}

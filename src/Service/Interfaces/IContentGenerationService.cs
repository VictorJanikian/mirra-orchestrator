using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IContentGenerationService
    {
        Task<WordpressBlogPostRequest> GenerateBlogPost(int schedulingId, Parameters parameters, CustomerPlatformConfiguration platformConfiguration, List<Content> lastPosts, IImageRepository imageRepository);
        Task<string> GenerateBlogPostSummary(int schedulingId, string originalPost, string summaryPrompt);

    }
}

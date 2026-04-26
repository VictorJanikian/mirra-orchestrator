using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IContentGenerationService
    {
        Task<WordpressBlogPost> GenerateBlogPost(Parameters parameters, CustomerPlatformConfiguration platformConfiguration, List<Content> lastPosts, IImageRepository imageRepository);
        Task<InstagramPost> GenerateInstagramPost(Parameters parameters, CustomerPlatformConfiguration platformConfiguration, List<Content> lastPosts, IImageRepository imageHosting);
        Task<string> GenerateBlogPostSummary(string originalPost, string summaryPrompt);

    }
}

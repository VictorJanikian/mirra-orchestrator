using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IContentGenerationService
    {
        Task<WordpressBlogPostRequest> GenerateBlogPost(int schedulingId, Parameters parameters, CustomerPlatformConfiguration platformConfiguration, ContentType contentType, List<Content> lastPosts, IImageRepository imageRepository);
        Task<string> GenerateBlogPostSummary(int schedulingId, string originalPost, string summaryPrompt);
        Task<InstagramPost> GenerateInstagramSinglePost(int schedulingId, Parameters parameters, ContentType contentType, List<Content> lastPosts);
        Task<InstagramPost> GenerateInstagramCarrousselPost(int schedulingId, Parameters parameters, ContentType contentType, List<Content> lastPosts);
    }
}

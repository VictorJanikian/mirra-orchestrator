using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IWordpressIntegration
    {
        public Task<string> SendBlogPostToWordpress(WordpressBlogPost blogPost, CustomerContentPlatformConfiguration configuration);
    }
}

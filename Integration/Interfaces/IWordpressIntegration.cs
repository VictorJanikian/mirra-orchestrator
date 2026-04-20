using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IWordpressIntegration : IImageRepository
    {
        public Task<string> SendBlogPostToWordpress(CustomerPlatformConfiguration platformConfiguration, WordpressBlogPost blogPost);

    }
}

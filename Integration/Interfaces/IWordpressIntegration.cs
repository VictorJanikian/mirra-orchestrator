using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IWordpressIntegration : IImageRepository
    {
        public CustomerPlatformTableRow _configuration { get; set; }

        public Task<string> SendBlogPostToWordpress(WordpressBlogPost blogPost);

    }
}

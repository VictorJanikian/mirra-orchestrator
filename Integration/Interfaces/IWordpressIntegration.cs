using Mirra_Orchestrator.Integration.Model.Request;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IWordpressIntegration
    {
        public Task WriteBlogPost(string url, WordpressBlogPost blogPost, string user, string password);
    }
}

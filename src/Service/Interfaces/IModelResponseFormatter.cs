using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IModelResponseFormatter
    {
        public WordpressBlogPostRequest GetWordpressBlogPostFromModelResponse(string modelResponse);
        public InstagramPost GetInstagramPostFromModelResponse(string modelResponse);
        Task<string> replaceImageMarkupsByImageLinks(string modelResponse, List<ImageInsideContent> imageAttributes);

    }
}

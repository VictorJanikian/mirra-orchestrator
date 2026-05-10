using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IModelResponseFormatter
    {
        public WordpressBlogPost GetWordpressBlogPostFromModelResponse(string modelResponse);
        Task<string> replaceImageMarkupsByImageLinks(string modelResponse, List<ImageInsideContent> imageAttributes);

    }
}

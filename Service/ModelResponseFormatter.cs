using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class ModelResponseFormatter : IModelResponseFormatter
    {
        public WordpressBlogPost GetWordpressBlogPostFromModelResponse(string modelResponse)
        {
            int divisorIndex = modelResponse.IndexOf("---");
            var postTitle = modelResponse.Substring(0, divisorIndex);
            var postContent = modelResponse.Substring(divisorIndex + 3);
            WordpressBlogPost wordpressBlogPost = new(postTitle, postContent);
            return wordpressBlogPost;
        }
    }
}

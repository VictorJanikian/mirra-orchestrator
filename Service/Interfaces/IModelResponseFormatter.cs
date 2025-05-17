using Mirra_Orchestrator.Integration.Model.Request;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IModelResponseFormatter
    {
        public WordpressBlogPost GetWordpressBlogPostFromModelResponse(string modelResponse);
    }
}

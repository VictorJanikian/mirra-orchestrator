using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IOrchestrationService
    {
        Task PostContent(Customer customer, ContentType contentType, Parameters parameters);
    }
}

using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IOrchestrationService
    {
        Task PostContent(Customer customer, ContentPlatform contentPlatform, Parameters parameters);
    }
}

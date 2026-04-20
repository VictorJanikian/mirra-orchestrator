using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IOrchestrationService
    {
        Task PostContent(Scheduling scheduling, Customer customer, Platform platform, Parameters parameters);
    }
}

using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IInstagramIntegration
    {
        Task<string> PublishImagePost(CustomerPlatformConfiguration platformConfiguration, string imageUrl, string caption);
    }
}

using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IPreviousContentRecoveryService
    {
        public Task<List<Content>> getLastContentsFrom(Customer customer, ContentType contentType);
    }
}

using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class PreviousContentRecoveryService : IPreviousContentRecoveryService
    {
        IContentRepository _contentRepository;

        public PreviousContentRecoveryService(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<List<Content>> getLastContentsFrom(CustomerContentPlatformConfiguration configurations)
        {
            return await _contentRepository.GetByCustomerAndPlatformConfiguration(configurations);
        }
    }
}

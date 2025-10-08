using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Repository.Interfaces
{
    public interface IContentRepository
    {
        Task<Content> Create(Content content);

        Task<List<Content>> GetByCustomerAndPlatformConfiguration(CustomerPlatformTableRow configuration);
    }
}

using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Repository.Interfaces
{
    public interface IModelConversationHistoryRepository
    {
        Task<ModelConversationHistory> Create(ModelConversationHistory modelConversationHistory);
    }
}

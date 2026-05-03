using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;
using Mirra_Orchestrator.Repository.Interfaces;

namespace Mirra_Orchestrator.Repository.Repositories
{
    public class ModelConversationHistoryRepository : DefaultRepository, IModelConversationHistoryRepository
    {
        public ModelConversationHistoryRepository(DatabaseContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<ModelConversationHistory> Create(ModelConversationHistory modelConversationHistory)
        {
            var row = _mapper.Map<ModelConversationHistoryTableRow>(modelConversationHistory);
            _context.ModelConversationHistories.Add(row);
            await _context.SaveChangesAsync();
            modelConversationHistory.Id = row.Id;
            return modelConversationHistory;
        }
    }
}

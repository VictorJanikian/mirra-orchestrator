using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class ModelConversationHistoryTableRowProfile : Profile
    {
        public ModelConversationHistoryTableRowProfile()
        {
            CreateMap<ModelConversationHistoryTableRow, ModelConversationHistory>();

            CreateMap<ModelConversationHistory, ModelConversationHistoryTableRow>()
                .AfterMap((entity, row) => row.CreatedAt = row.CreatedAt ?? DateTime.Now);
        }
    }
}

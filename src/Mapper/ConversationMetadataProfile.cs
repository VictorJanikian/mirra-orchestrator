using AutoMapper;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Mapper
{
    public class ConversationMetadataProfile : Profile
    {
        public ConversationMetadataProfile()
        {

            CreateMap<ConversationMetadata, ModelConversationHistory>();
        }

    }
}

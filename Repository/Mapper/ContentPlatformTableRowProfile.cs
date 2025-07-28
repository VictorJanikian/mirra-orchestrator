using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class ContentPlatformTableRowProfile : Profile
    {
        public ContentPlatformTableRowProfile()
        {
            CreateMap<ContentPlatformTableRow, ContentPlatform>();
        }
    }
}

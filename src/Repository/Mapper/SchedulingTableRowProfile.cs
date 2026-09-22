using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class SchedulingTableRowProfile : Profile
    {
        public SchedulingTableRowProfile()
        {
            CreateMap<SchedulingTableRow, Scheduling>()
                .ForMember(destination => destination.HasInstagramAIGeneratedLabel,
                    options => options.MapFrom(source => source.InstagramAIGeneratedLabel == 1));
        }
    }
}

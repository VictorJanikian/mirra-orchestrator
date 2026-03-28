using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class SchedulingStatusTableRowProfile : Profile
    {
        public SchedulingStatusTableRowProfile()
        {
            CreateMap<SchedulingStatusTableRow, SchedulingStatus>();
            CreateMap<SchedulingStatus, SchedulingStatusTableRow>();
        }
    }
}

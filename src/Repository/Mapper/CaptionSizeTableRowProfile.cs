using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class CaptionSizeTableRowProfile : Profile
    {
        public CaptionSizeTableRowProfile()
        {
            CreateMap<CaptionSizeTableRow, CaptionSize>();
            CreateMap<CaptionSize, CaptionSizeTableRow>();
        }
    }
}

using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class CustomerContentPlatformTableRowProfile : Profile
    {
        public CustomerContentPlatformTableRowProfile()
        {
            CreateMap<CustomerContentPlatformConfigurationTableRow, CustomerContentPlatformConfiguration>();
        }
    }
}

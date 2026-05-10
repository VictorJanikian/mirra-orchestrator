using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class CustomerTableRowProfile : Profile
    {
        public CustomerTableRowProfile()
        {
            CreateMap<CustomerTableRow, Customer>();
        }
    }
}

using AutoMapper;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;

namespace Mirra_Orchestrator.Repository.Mapper
{
    public class ContentTableRowProfile : Profile
    {
        public ContentTableRowProfile()
        {
            CreateMap<Content, ContentTableRow>()
                .ForMember(row => row.ContentType, options => options.Ignore())
                .ForMember(row => row.Customer, options => options.Ignore())
                .ForMember(row => row.Parameter, options => options.Ignore())
                .ForMember(row => row.CreatedAt, options => options.Ignore())
                .ForMember(row => row.ContentTypeId, options => options.MapFrom(entity => entity.ContentType.Id))
                .ForMember(row => row.ParameterId, options => options.MapFrom(entity => entity.Parameters.Id))
                .ForMember(row => row.CustomerId, options => options.MapFrom(entity => entity.Customer.Id))
                .AfterMap((entity, row) => row.CreatedAt = row.CreatedAt ?? DateTime.Now);

            CreateMap<ContentTableRow, Content>();

        }
    }
}

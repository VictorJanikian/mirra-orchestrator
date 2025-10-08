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
                .ForMember(row => row.CustomerPlatformConfiguration, options => options.Ignore())
                .ForMember(row => row.Parameter, options => options.Ignore())
                .ForMember(row => row.CreatedAt, options => options.Ignore())
                .ForMember(row => row.ParameterId, options => options.MapFrom(entity => entity.Parameters.Id))
                .ForMember(row => row.CustomerPlatformConfigurationId, options => options.MapFrom(entity => entity.CustomerPlatformConfiguration.Id))
                .AfterMap((entity, row) => row.CreatedAt = row.CreatedAt ?? DateTime.Now);

            CreateMap<ContentTableRow, Content>();

        }
    }
}

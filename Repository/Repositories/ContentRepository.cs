using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.DbEntities;
using Mirra_Orchestrator.Repository.Interfaces;

namespace Mirra_Orchestrator.Repository.Repositories
{
    public class ContentRepository : DefaultRepository, IContentRepository
    {
        public ContentRepository(DatabaseContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<Content> Create(Content content)
        {
            var row = _mapper.Map<ContentTableRow>(content);
            _context.Contents.Add(row);
            await _context.SaveChangesAsync();
            content.Id = row.Id;
            return content;
        }

        public async Task<List<Content>> GetByCustomerAndPlatformConfiguration(CustomerPlatformTableRow configuration)
        {
            return await _context
                   .Contents
                   .AsNoTracking()
                   .Where(content => content.CustomerPlatformConfigurationId == configuration.Id)
                   .OrderByDescending(content => content.CreatedAt)
                   .Take(10)
                   .ProjectTo<Content>(_mapper.ConfigurationProvider)
                   .ToListAsync();
        }
    }
}

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

        public async Task<List<Content>> GetByCustomerAndContentType(Customer customer, ContentType contentType)
        {
            return await _context
                   .Contents
                   .AsNoTracking()
                   .Where(content => content.CustomerId == customer.Id && content.ContentTypeId == contentType.Id)
                   .ProjectTo<Content>(_mapper.ConfigurationProvider)
                   .ToListAsync();
        }
    }
}

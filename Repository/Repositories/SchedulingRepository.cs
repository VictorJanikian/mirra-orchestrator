using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.Interfaces;

namespace Mirra_Orchestrator.Repository.Repositories
{
    public class SchedulingRepository : DefaultRepository, ISchedulingRepository
    {
        public SchedulingRepository(DatabaseContext context, IMapper mapper) : base(context, mapper)
        { }

        public async Task<List<Scheduling>> GetAllSchedulings()
        {
            return await _context
                .Schedulings
                .AsNoTracking()
                .ProjectTo<Scheduling>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}

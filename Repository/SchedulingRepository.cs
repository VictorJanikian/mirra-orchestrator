using Microsoft.EntityFrameworkCore;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.Interfaces;

namespace Mirra_Orchestrator.Repository
{
    public class SchedulingRepository : ParentRepository, ISchedulingRepository
    {
        public SchedulingRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Scheduling>> GetAllSchedulings()
        {
            return await _context.Schedulings.AsNoTracking().ToListAsync();
        }
    }
}

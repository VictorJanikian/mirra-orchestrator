using AutoMapper;

namespace Mirra_Orchestrator.Repository.Repositories
{
    public class DefaultRepository
    {
        protected DatabaseContext _context;
        protected readonly IMapper _mapper;

        public DefaultRepository(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirra_Orchestrator.Repository
{
    public class ParentRepository
    {
        protected DatabaseContext _context;

        public ParentRepository(DatabaseContext context) { _context = context; }

    }
}

using Mirra_Orchestrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirra_Orchestrator.Repository.Interfaces
{
    public interface ISchedulingRepository
    {
        Task<List<Scheduling>> GetAllSchedulings();
    }
}

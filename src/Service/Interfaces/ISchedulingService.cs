using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface ISchedulingService
    {
        public Task runAllScheduledPosts();
    }
}

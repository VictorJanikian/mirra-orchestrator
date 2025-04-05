using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator
{
    public class Scheduler
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly ISchedulingService _schedulingService;

        public Scheduler(ILogger<Scheduler> logger, ISchedulingService schedulingService)
        {
            _logger = logger;
            _schedulingService = schedulingService;
        }

        [Function("MirraScheduler")]
        public async Task MirraScheduler([TimerTrigger("* * * * *")] TimerInfo timerInfo,
    FunctionContext context)
        {
            await _schedulingService.runAllScheduledPosts();
        }
    }
}

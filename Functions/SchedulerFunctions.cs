using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Functions
{
    public class SchedulerFunctions
    {
        private readonly ILogger<SchedulerFunctions> _logger;
        private readonly ISchedulingService _schedulingService;

        public SchedulerFunctions(ILogger<SchedulerFunctions> logger, ISchedulingService schedulingService)
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

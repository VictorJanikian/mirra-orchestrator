using Microsoft.AspNetCore.Http;
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
        public async Task MirraScheduler([TimerTrigger("*/15 * * * *")] TimerInfo timerInfo,
    FunctionContext context)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await _schedulingService.runAllScheduledPosts();
        }

        /*[Function("RunSchedulerManually")]
        public async Task<IResult> RunSchedulerManually(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "scheduler/run")] HttpRequest request)
        {
            _logger.LogInformation($"Execução manual do scheduler iniciada em: {DateTime.Now}");
            await _schedulingService.runAllScheduledPosts();

            return Results.NoContent();
        }*/
    }
}

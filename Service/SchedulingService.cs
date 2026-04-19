using Microsoft.Extensions.Logging;
using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Helpers;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;
using NCrontab;

namespace Mirra_Orchestrator.Service
{
    public class SchedulingService : ISchedulingService
    {
        private ISchedulingRepository _schedulingRepository;
        private IOrchestrationService _orchestrationService;
        private ILogger<SchedulingService> _logger;
        public SchedulingService(ISchedulingRepository repository,
            IOrchestrationService orchestrationService,
            ILogger<SchedulingService> logger)
        {
            _schedulingRepository = repository;
            _orchestrationService = orchestrationService;
            _logger = logger;
        }

        public async Task runAllScheduledPosts()
        {
            var schedulings = await _schedulingRepository.GetAllSchedulings();

            foreach (var scheduling in schedulings)
            {
                try
                {
                    bool shouldRun = ShouldExecuteNow(scheduling);
                    if (shouldRun)
                        await _orchestrationService.PostContent(scheduling.CustomerPlatformConfiguration.Customer,
                                                                scheduling.CustomerPlatformConfiguration.Platform,
                                                                scheduling.Parameters);
                }

                catch (System.Exception e)
                {
                    _logger.LogInformation(e.Message + " " + e.StackTrace);
                }

            }
        }

        private bool ShouldExecuteNow(Scheduling scheduling)
        {
            if (scheduling.SchedulingStatus.Id != (int)ESchedulingStatus.ACTIVE)
                return false;

            var cronExpression = CronTimezoneHelper.ConvertUtcToTimezone(scheduling.Interval, scheduling.Timezone);
            var tz = TimeZoneInfo.FindSystemTimeZoneById(scheduling.Timezone);
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            var schedule = CrontabSchedule.Parse(cronExpression);

            DateTime nextOccurrence = schedule.GetNextOccurrence(now.AddMinutes(-now.Minute - 1)); // Ignorando os minutos

            return nextOccurrence.Hour == now.Hour && nextOccurrence.Day == now.Day && nextOccurrence.DayOfWeek == now.DayOfWeek;
        }
    }
}

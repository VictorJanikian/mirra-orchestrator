using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;
using NCrontab;

namespace Mirra_Orchestrator.Service
{
    public class SchedulingService : ISchedulingService
    {
        private ISchedulingRepository _schedulingRepository;
        private IOrchestrationService _orchestrationService;
        public SchedulingService(ISchedulingRepository repository, IOrchestrationService orchestrationService)
        {
            _schedulingRepository = repository;
            _orchestrationService = orchestrationService;
        }

        public async Task runAllScheduledPosts()
        {
            try
            {
                var schedulings = await _schedulingRepository.GetAllSchedulings();

                foreach (var scheduling in schedulings)
                {
                    bool shouldRun = ShouldExecuteNow(scheduling.Interval);
                    if (shouldRun)
                    {
                        await _orchestrationService.PostContent(scheduling.Customer, scheduling.ContentType, scheduling.Parameters);
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.Write(e.Message);
            }
        }

        private bool ShouldExecuteNow(string cronExpression)
        {
            var now = DateTime.UtcNow;

            var schedule = CrontabSchedule.Parse(cronExpression);

            DateTime nextOccurrence = schedule.GetNextOccurrence(now.AddMinutes(-now.Minute - 1)); // Ignorando os minutos

            return nextOccurrence.Hour == now.Hour && nextOccurrence.Day == now.Day && nextOccurrence.DayOfWeek == now.DayOfWeek;
        }
    }
}

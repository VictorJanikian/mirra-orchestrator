using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;
using NCrontab;

namespace Mirra_Orchestrator.Service
{
    public class SchedulingService : ISchedulingService
    {
        private ISchedulingRepository _schedulingRepository;
        private IWordpressIntegration _wordpressIntegration;
        public SchedulingService(ISchedulingRepository repository, IWordpressIntegration wordpressIntegration)
        {
            _schedulingRepository = repository;
            _wordpressIntegration = wordpressIntegration;
        }

        public async Task runAllScheduledPosts()
        {
            var schedulings = await _schedulingRepository.GetAllSchedulings();
            foreach (var scheduling in schedulings)
            {
                bool shouldRun = ShouldExecuteNow(scheduling.Interval);
                if (shouldRun)
                {
                    WordpressBlogPost blogPost = new WordpressBlogPost("Hello", "World");
                    await _wordpressIntegration.WriteBlogPost(scheduling.Url, blogPost, scheduling.User, scheduling.Password);
                }
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

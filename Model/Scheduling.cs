namespace Mirra_Orchestrator.Model
{
    public class Scheduling : Entity
    {
        public CustomerContentPlatformConfiguration CustomerContentPlatformConfiguration { get; set; }
        public Parameters Parameters { get; set; }
        public string Interval { get; set; }

    }
}

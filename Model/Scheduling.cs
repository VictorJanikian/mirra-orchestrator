namespace Mirra_Orchestrator.Model
{
    public class Scheduling : Entity
    {
        public CustomerContentTypeConfiguration CustomerContentTypeConfigurations { get; set; }
        public Parameters Parameters { get; set; }
        public string Interval { get; set; }

    }
}

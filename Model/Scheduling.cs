namespace Mirra_Orchestrator.Model
{
    public class Scheduling : Entity
    {
        public Customer Customer { get; set; }
        public Parameters Parameters { get; set; }
        public string Interval { get; set; }
        public ContentType ContentType { get; set; }


    }
}

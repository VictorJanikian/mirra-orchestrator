namespace Mirra_Orchestrator.Model
{
    public class ContentType : Entity
    {
        public string Name { get; set; }
        public string Prompt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

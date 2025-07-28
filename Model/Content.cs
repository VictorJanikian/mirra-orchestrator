namespace Mirra_Orchestrator.Model
{
    public class Content : Entity
    {
        public CustomerContentPlatformConfiguration CustomerContentPlatformConfiguration { get; set; }
        public Parameters Parameters { get; set; }
        public string ContentTitle { get; set; }
        public string ContentUrl { get; set; }
        public string ContentSummary { get; set; }

    }
}

namespace Mirra_Orchestrator.Model
{
    public class Platform : Entity
    {
        public string Name { get; set; } = null!;
        public string? Prompt { get; set; }
        public string? SummaryPrompt { get; set; }
        public string SystemPrompt { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}

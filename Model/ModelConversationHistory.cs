namespace Mirra_Orchestrator.Model
{
    public class ModelConversationHistory : Entity
    {
        public string? SystemPrompt { get; set; }
        public string Prompt { get; set; }
        public string ModelResponse { get; set; }
        public int SchedulingId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

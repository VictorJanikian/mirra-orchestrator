using Mirra_Orchestrator.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("model_conversation_history")]
    public class ModelConversationHistoryTableRow : Entity
    {
        public string? SystemPrompt { get; set; }
        public string Prompt { get; set; }
        public string ModelResponse { get; set; }
        public SchedulingTableRow Scheduling { get; set; }
        public int? SchedulingId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

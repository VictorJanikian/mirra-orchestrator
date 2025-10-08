using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("platforms")]
    public class PlatformTableRow : EntityTableRow
    {
        public string Name { get; set; }
        public string? Prompt { get; set; }
        public string? SummaryPrompt { get; set; }
        public string SystemPrompt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("content_types")]
    public class ContentTypeTableRow : EntityTableRow
    {
        public string Name { get; set; }
        public string? Prompt { get; set; }
        public string? SummaryPrompt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

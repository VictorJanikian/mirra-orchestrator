using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("contents")]
    public class ContentTableRow : EntityTableRow
    {
        public int CustomerContentPlatformConfigurationId { get; set; }
        public CustomerContentPlatformConfigurationTableRow CustomerContentPlatformConfiguration { get; set; }
        public int ParameterId { get; set; }
        public ParametersTableRow Parameter { get; set; }
        public string? ContentTitle { get; set; }
        public string ContentUrl { get; set; }
        public string ContentSummary { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("schedulings")]
    public class SchedulingTableRow : EntityTableRow
    {
        public int CustomerContentPlatformConfigurationId { get; set; }
        public CustomerContentPlatformConfigurationTableRow CustomerContentPlatformConfiguration { get; set; }
        public int ParameterId { get; set; }
        public ParametersTableRow Parameters { get; set; }
        public string Interval { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

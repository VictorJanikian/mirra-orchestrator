using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("customers")]
    public class CustomerTableRow : EntityTableRow
    {
        public string Name { get; set; }
        public string? Email { get; set; }
        public List<CustomerContentPlatformConfigurationTableRow> CustomerContentPlatforms { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("customer_content_platforms_configurations")]
    public class CustomerContentPlatformConfigurationTableRow : EntityTableRow
    {
        public int CustomerId { get; set; }
        public CustomerTableRow Customer { get; set; }
        public int ContentPlatformId { get; set; }
        public ContentPlatformTableRow ContentPlatform { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

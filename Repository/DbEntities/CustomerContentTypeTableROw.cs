using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("customer_content_types")]
    public class CustomerContentTypeTableRow : EntityTableRow
    {
        public int CustomerId { get; set; }
        public CustomerTableRow Customer { get; set; }
        public int ContentTypeId { get; set; }
        public ContentTypeTableRow ContentType { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

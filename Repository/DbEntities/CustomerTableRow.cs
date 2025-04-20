using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("customers")]
    public class CustomerTableRow : EntityTableRow
    {
        public string UniqueNumber { get; set; }
        public List<CustomerContentTypeTableRow> CustomerContentTypes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("contents")]
    public class ContentTableRow : EntityTableRow
    {
        public int CustomerId { get; set; }
        public CustomerTableRow Customer { get; set; }
        public int ParameterId { get; set; }
        public ParametersTableRow Parameter { get; set; }
        public int ContentTypeId { get; set; }
        public ContentTypeTableRow ContentType { get; set; }
        public string ContentUrl { get; set; }
        public string ContentSummary { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

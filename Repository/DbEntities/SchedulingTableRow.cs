using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("schedulings")]
    public class SchedulingTableRow : EntityTableRow
    {
        public int CustomerId { get; set; }
        public CustomerTableRow Customer { get; set; }
        public int ParameterId { get; set; }
        public ParametersTableRow Parameters { get; set; }
        public string Interval { get; set; }
        public int ContentTypeId { get; set; }
        public ContentTypeTableRow ContentType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

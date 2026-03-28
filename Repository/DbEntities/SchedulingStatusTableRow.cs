using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("scheduling_status")]
    public class SchedulingStatusTableRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

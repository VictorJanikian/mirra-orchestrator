using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("platforms")]
    public class PlatformTableRow : EntityTableRow
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

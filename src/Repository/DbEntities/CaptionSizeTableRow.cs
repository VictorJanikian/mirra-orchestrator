using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("caption_sizes")]
    public class CaptionSizeTableRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

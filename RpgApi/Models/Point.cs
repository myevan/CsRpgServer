using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rpg.Models
{
    [Table("Point")]
    public class Point
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PlayerId")]
        public required Player Player { get; set; }
        public required int Num { get; set; }
        public int Amount { get; set; }
    }
}

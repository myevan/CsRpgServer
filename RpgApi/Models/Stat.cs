using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rpg.Models
{
    [Table("Stat")]
    public class Stat
    {
        [Key]
        public int Id { get; set; }

        public required int Num { get; set; }
        public int Lv { get; set; } = 1;

        public int PlayerId { get; set; }
    }
}

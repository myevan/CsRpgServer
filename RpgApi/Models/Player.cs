using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rpg.Models
{
    [Table("Player")]
    public class Player
    {
        [Key]
        public int Id { get; set; }
    }
}

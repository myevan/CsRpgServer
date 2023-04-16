using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rpg.Models
{
    [Index(nameof(Guid))]
    [Table("Player")]
    
    public class Player
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;

        public required string Guid { get; set; }

        public string Name { get; set; } = "";

        public int RealCash { get; set; } = 0;
        public int FreeCash { get; set; } = 0;

        public virtual ICollection<Point> Points { get; set; } = new List<Point>();
    }
}
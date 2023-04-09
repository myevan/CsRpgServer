using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rpg.Models
{
    [Table("Player")]
    [Index(nameof(Guid), IsUnique = true)]
    public class Player
    {
        [Key]
        public int Id { get; set; }

        public required string Guid { get; set; }

        public required string Name { get; set; }

        public int RealCash { get; set; } = 0;
        public int FreeCash { get; set; } = 0;
    }
}

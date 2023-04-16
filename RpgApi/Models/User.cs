using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rpg.Models
{
    [Index(nameof(Name))]
    [Table("User")]

    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default;

        public required string Name { get; set; }
        public required string Guid { get; set; }
        public string Password { get; set; } = "";

        public DateTime Created { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rpg.Models
{
    [Table("Account")]

    public class Account
    {
        [Key]
        public required string Key { get; set; }
        public required string Secret { get; set; }

        public required virtual Player Player { get; set; }

    }
}

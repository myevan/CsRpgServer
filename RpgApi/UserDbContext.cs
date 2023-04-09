using Microsoft.EntityFrameworkCore;
using Rpg.Models;

namespace Rpg
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> opts) : base(opts)
        {
        }


        public async Task<Player?> FindPlayerAsync(int id)
        {
            return await PlayerSet.FindAsync(id);
        }

        public async Task<Player> TouchPlayerAsync(string inGuid)
        {
            var oldPlayer = PlayerSet.Where(player => player.Guid == inGuid).FirstOrDefault();
            if (oldPlayer != null)
            {
                return oldPlayer;
            }

            Player newPlayer = new Player()
            {
                Guid = inGuid,
                Name = "",
                FreeCash = 100,
            };
            Add(newPlayer);
            await SaveChangesAsync();
            return newPlayer;
        }

        public DbSet<Player> PlayerSet { get; set; }
    }
}

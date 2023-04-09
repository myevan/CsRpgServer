using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rpg.Models;

namespace Rpg
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> opts) : base(opts)
        {
        }

        public async Task<Player?> FindPlayer(int id)
        {
            return await PlayerSet.FindAsync(id);
        }

        public async Task<Player> MakePlayer()
        {
            Player newPlayer = new Player();
            Add(newPlayer);
            await SaveChangesAsync();
            return newPlayer;
        }

        public DbSet<Player> PlayerSet { get; set; }
    }
}

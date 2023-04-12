using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Rpg.Models;

namespace Rpg
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> opts) : base(opts)
        {
        }

        public async Task<Account?> FindAccountAsync(int inAccountId)
        {
            return await AccountSet.FindAsync(inAccountId);
        }


        public async Task<Account?> TouchAccountAsync(string inKey, string inSecret, Func<Player> createPlayer)
        {
            var oldAccount = await AccountSet.FindAsync(inKey);
            if (oldAccount != null)
            {
                if (oldAccount.Secret != inSecret)
                {
                    return null;
                }
            }

            var newPlayer = createPlayer();
            var newAccount = new Account() { Key = inKey, Secret = inSecret, Player = newPlayer };
            Add(newPlayer);
            Add(newAccount);
            await SaveChangesAsync();
            return newAccount;
        }

        public async Task<Player?> FindPlayerAsync(int inPlayerId)
        {
            return await PlayerSet.FindAsync(inPlayerId);
        }
 
        public async Task<Point> TouchPointAsync(Player inPlayer, int inNum)
        {
            var oldPoint = PointSet.Where(each => each.Player.Id == inPlayer.Id && each.Num == inNum).FirstOrDefault();
            if (oldPoint != null)
            {
                return oldPoint;
            }

            var newPoint = new Point()
            {
                Player = inPlayer,
                Num = inNum,
            };
            Add(newPoint);
            await SaveChangesAsync();
            return newPoint;
        }

        public async Task<List<Point>> GetPointListAsync(Player inPlayer)
        {
            return await PointSet.Where(each => each.Player.Id == inPlayer.Id).ToListAsync();
        }

        public DbSet<Account> AccountSet { get; set; }
        public DbSet<Player> PlayerSet { get; set; }
        public DbSet<Point> PointSet { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Rpg.Models;

namespace Rpg.DbContexts
{
    public class WorldDbContext :DbContext
    {
        public WorldDbContext(DbContextOptions<WorldDbContext> opts) : base(opts)
        {
        }
        
        public async Task<Player?> TouchPlayerAsync(string inGuid)
        {
            var oldPlayer = await PlayerSet.Where(each => each.Guid == inGuid).FirstOrDefaultAsync();
            if (oldPlayer != null)
            {
                return oldPlayer;
            }

            var newPlayer = new Player() { Guid = inGuid };
            Add(newPlayer);
            await SaveChangesAsync();
            return newPlayer;
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

        public DbSet<Player> PlayerSet { get; set; }
        public DbSet<Point> PointSet { get; set; }
    }
}

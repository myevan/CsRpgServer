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
            var oldPlayer = PlayerSet.Where(each => each.Guid == inGuid).FirstOrDefault();
            if (oldPlayer != null)
            {
                return oldPlayer;
            }

            var newPlayer = new Player()
            {
                Guid = inGuid
            };
            Add(newPlayer);

            // TODO: 필수 포인트 생성
            for (int iNum = 1; iNum < 3; ++iNum)
            {
                var newPoint = new Point()
                {
                    Player = newPlayer,
                    Num = iNum,
                };
                Add(newPoint);
            }

            await SaveChangesAsync();
            return newPlayer;
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
            return PointSet.Where(each => each.Player.Id == inPlayer.Id).ToList();
        }

        public DbSet<Player> PlayerSet { get; set; }
        public DbSet<Point> PointSet { get; set; }
    }
}

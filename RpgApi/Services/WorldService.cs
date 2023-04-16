using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Rpg.DbContexts;
using Rpg.Helpers;
using Rpg.Models;

namespace Rpg.Services
{
    public class WorldService
    {
        public WorldService(WorldDbContext dbCtx, JwtService jwtSvc, IDistributedCache distCache)
        {
            _dbCtx = dbCtx;
            _jwtSvc = jwtSvc;
            _distCache = distCache;
        }

        public async Task<Player> ConnectPlayerAsync(Session ses)
        {            
            var sesUserGuid = ses.GetUserGuid();
            var ctxPlayer = ValidPlayer(await _dbCtx.TouchPlayerAsync(sesUserGuid));
            ses.SetPlayerId(ctxPlayer.Id);
            return ctxPlayer;
        }

        public async Task<Player> LoadPlayerAsync(Session ses)
        {
            var sesPlayerId = ses.GetPlayerId();
            var ctxPlayer = ValidPlayer(await _dbCtx.FindPlayerAsync(sesPlayerId));
            return ctxPlayer;
        }

        public async Task<Player> ChangePlayerNameAsync(Session ses, string inName)
        {
            var ctxPlayer = await LoadPlayerAsync(ses);
            ctxPlayer.Name = inName;
            _dbCtx.SaveChanges();

            return ctxPlayer;
        }

        public async Task<Stat> IncPlayerStatAsync(Session ses, int inNum, int inOldLv, int inNewLv)
        {
            int prtGoldCost = 100;
            var ctxPlayer = await LoadPlayerAsync(ses);
            ValidHelper.PlayeGoldCost(ctxPlayer, prtGoldCost);
            ctxPlayer.Gold -= prtGoldCost;

            var ctxStat = await _dbCtx.TouchStatAsync(ctxPlayer.Id, inNum);
            ValidHelper.StatLevel(ctxStat, inOldLv);
            ctxStat.Lv = inNewLv;

            await _dbCtx.SaveChangesAsync();
            return ctxStat;
        }

        private static Player ValidPlayer(Player? obj) => ValidHelper.Object("PLAYER", obj);

        private readonly WorldDbContext _dbCtx;
        private readonly JwtService _jwtSvc;
        private readonly IDistributedCache _distCache;
    }
}

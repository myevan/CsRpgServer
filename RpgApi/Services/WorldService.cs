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

        public async Task<Player> ConnectPlayerAsync(HttpContext httpCtx)
        {
            var ses = ValidSession(httpCtx);
            var sesUserGuid = ses.GetUserGuid();
            var ctxPlayer = ValidPlayer(await _dbCtx.TouchPlayerAsync(sesUserGuid));
            ses.SetPlayerId(ctxPlayer.Id);
            return ctxPlayer;
        }
        
        public async Task<Player> ChangePlayerNameAsync(HttpContext httpCtx, string inName)
        {
            var ses = ValidSession(httpCtx);
            var sesPlayerId = ses.GetPlayerId();
            var ctxPlayer = ValidPlayer(await _dbCtx.FindPlayerAsync(sesPlayerId));
            ctxPlayer.Name = inName;
            _dbCtx.SaveChanges();

            return ctxPlayer;
        }
        private Session ValidSession(HttpContext httpCtx)
        {
            var sesKey = ValidHelper.String("CLAIM_SESSION_KEY", _jwtSvc.LoadClaimSessionKey(httpCtx));
            return Session.Load(_distCache, sesKey);
        }

        private static Player ValidPlayer(Player? obj) => ValidHelper.Object("PLAYER", obj);

        private readonly WorldDbContext _dbCtx;
        private readonly JwtService _jwtSvc;
        private readonly IDistributedCache _distCache;
    }
}

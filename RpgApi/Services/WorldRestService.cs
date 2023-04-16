using Microsoft.EntityFrameworkCore;
using Rpg.DbContexts;

namespace Rpg.Services
{
    public class WorldRestService
    {
        public WorldRestService(WorldService worldSvc, JwtService jwtSvc, WorldDbContext dbCtx)
        {            
            _worldSvc = worldSvc;
            _jwtSvc = jwtSvc;
            _dbCtx = dbCtx;
        }

        public async Task<IResult> PostWorldPlayerAsync(HttpContext httpCtx)
        {
            var ses = _jwtSvc.LoadSession(httpCtx);
            if (ses == null) return Results.NotFound();
            var player = await _worldSvc.ConnectPlayerAsync(ses);
            return Results.Ok(player);
        }

        public async Task<IResult> GetWorldPlayerAsync(HttpContext httpCtx)
        {
            var ses = _jwtSvc.LoadSession(httpCtx);
            if (ses == null) return Results.NotFound();
            int sesPlayerId = ses.GetPlayerId();
            return await GetWorldPlayerAsync(sesPlayerId);
        }

        public async Task<IResult> GetWorldPlayerAsync(int inPlayerId)
        {
            var player = await _dbCtx.FindPlayerAsync(inPlayerId);
            if (player == null) return Results.NotFound();
            return Results.Ok(player);
        }

        public async Task<IResult> GetWorldPlayersAsync()
        {
            var players = await _dbCtx.PlayerSet.ToListAsync();
            return Results.Ok(players);
        }

        private readonly WorldService _worldSvc;
        private readonly JwtService _jwtSvc;
        private readonly WorldDbContext _dbCtx;
    }
}

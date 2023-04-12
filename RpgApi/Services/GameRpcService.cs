using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Rpg.Grpc;
using Rpg.Helpers;
using Rpg.Models;
using System;
using System.Xml.Linq;

namespace Rpg.Services
{
    public class GameRpcService : Game.GameBase
    {
        public GameRpcService(UserDbContext dbCtx, AuthTokenService authSvc, IMapper mapper)
        {
            _dbCtx = dbCtx;
            _authSvc = authSvc;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async override Task<WorldResponse> EnterWorld(WorldRequest req, ServerCallContext ctx)
        {
            ValidGuid(req.Guid);

            var ctxCount = ValidAccount(await _dbCtx.TouchAccountAsync(req.Guid, "SECRET", () => new Player()));
            var ctxPlayer = ctxCount.Player;
            var newToken = _authSvc.CreatePlayerToken(ctxPlayer.Id);
            
            var res = new WorldResponse()
            {
                Token = newToken,
                Player = _mapper.Map<PlayerResponse>(ctxPlayer),
            };

            foreach (var eachPoint in await _dbCtx.GetPointListAsync(ctxPlayer))
            {
                res.Points.Add(_mapper.Map<PointResponse>(eachPoint));
            }
            
            return await Task.FromResult(res);
        }

        public async override Task<PlayerResponse> ChangePlayerName(NameRequest req, ServerCallContext ctx)
        {
            ValidName(req.Name, maxLen: 8);

            var ctxPlayer = await ValidGetPlayerAsync(ctx);
            ctxPlayer.Name = req.Name;
            _dbCtx.SaveChanges();

            var res = _mapper.Map<PlayerResponse>(ctxPlayer);
            return await Task.FromResult(res);
        }

        private async Task<Player> ValidGetPlayerAsync(ServerCallContext ctx)
        {
            var httpCtx = ctx.GetHttpContext();
            var authPlayerId = _authSvc.GetPlayerId(httpCtx);
            var ctxPlayer = ValidPlayer(await _dbCtx.FindPlayerAsync(authPlayerId));
            return ctxPlayer;
        }


        private static string ValidGuid(string val) => ValidHelper.String("GUID", val);
        private static string ValidName(string val, int maxLen) => ValidHelper.StringLength("NAME", val, maxLen: maxLen);

        private static Account ValidAccount(Account? obj) => ValidHelper.Object<Account>("ACCOUNT", obj);

        private static Player ValidPlayer(Player? obj) => ValidHelper.Object<Player>("PLAYER", obj);

        private UserDbContext _dbCtx;
        private AuthTokenService _authSvc;
        private IMapper _mapper;
    }
}

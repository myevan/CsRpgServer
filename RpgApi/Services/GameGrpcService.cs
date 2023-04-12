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
    public class GameGrpcService : Game.GameBase
    {
        public GameGrpcService(GameService gameSvc, JwtAuthService authSvc, IMapper mapper)
        {
            _gameSvc = gameSvc;
            _authSvc = authSvc;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async override Task<WorldResponse> EnterWorld(WorldRequest req, ServerCallContext ctx)
        {
            var ctxPlayer = _gameSvc.EnterWorld(req.Guid);
            var newToken = _authSvc.CreatePlayerToken(ctxPlayer.Id);
            
            var res = new WorldResponse()
            {
                Token = newToken,
                Player = _mapper.Map<PlayerResponse>(ctxPlayer),
            };

            /* TODO
            foreach (var eachPoint in await ctxPlayer.Points)
            {
                res.Points.Add(_mapper.Map<PointResponse>(eachPoint));
            }
            */
            
            return await Task.FromResult(res);
        }

        public async override Task<PlayerResponse> ChangePlayerName(NameRequest req, ServerCallContext ctx)
        {
            var authPlayerId = GetAuthPlayerId(ctx);
            var ctxPlayer = _gameSvc.ChangePlayerName(authPlayerId, req.Name);
            
            var res = _mapper.Map<PlayerResponse>(ctxPlayer);
            return await Task.FromResult(res);
        }

        private int GetAuthPlayerId(ServerCallContext ctx)
        {
            var httpCtx = ctx.GetHttpContext();
            var authPlayerId = _authSvc.GetPlayerId(httpCtx);
            return authPlayerId;
        }

        private GameService _gameSvc;
        private JwtAuthService _authSvc;
        private IMapper _mapper;
    }
}

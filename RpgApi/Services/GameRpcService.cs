using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Rpg.Grpc;
using Rpg.Models;
using System;

namespace Rpg.Services
{
    public class GameRpcService : Game.GameBase
    {
        public GameRpcService(UserDbContext dbCtx, AuthService authSvc, IMapper mapper)
        {
            _dbCtx = dbCtx;
            _authSvc = authSvc;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public async override Task<WorldResponse> EnterWorld(WorldRequest req, ServerCallContext ctx)
        {
            VerifyGuid(req.Guid, "RPC_ERR_REQ_PLAYER_GUID");

            var reqPlayer = VerifyModel(await _dbCtx.TouchPlayerAsync(req.Guid), "RPC_ERR_REQ_PLAYER");
            
            var newToken = _authSvc.CreateToken(reqPlayer.Id);
            var res = new WorldResponse()
            {
                Token = newToken,
                Player = _mapper.Map<PlayerResponse>(reqPlayer),
            };
            return await Task.FromResult(res);
        }

        public async override Task<PlayerResponse> ChangePlayerName(NameRequest req, ServerCallContext ctx)
        {
            VerifyString(req.Name, "RPC_ERR_REQ_NAME");
            VerifyStringLength(req.Name, "RPC_ERR_REQ_NAME_TOO_LONG", maxLen: 8);

            var ctxPlayer = VerifyModel(await GetContextPlayer(ctx), "RPC_ERR_CTX_PLAYER");
            ctxPlayer.Name = req.Name;

            _dbCtx.SaveChanges();

            var res = _mapper.Map<PlayerResponse>(ctxPlayer);
            return await Task.FromResult(res);
        }

        private async Task<Player?> GetContextPlayer(ServerCallContext ctx)
        {
            var httpCtx = ctx.GetHttpContext();
            return await _authSvc.GetPlayer(httpCtx, _dbCtx);
        }

        private static string VerifyGuid(string val, string detail)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, detail));
            }
            return val;
        }
        private static string VerifyString(string val, string detail)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, detail));
            }
            return val;
        }

        private static string VerifyStringLength(string val, string detail, int maxLen)
        {
            if (maxLen > 0 && val.Length > maxLen)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, detail));
            }
            return val;
        }

        private static T VerifyModel<T>(T? val, string detail)
        {
            if (val == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, detail));
            }
            return val;
        }

        private UserDbContext _dbCtx;
        private AuthService _authSvc;
        private IMapper _mapper;
    }
}

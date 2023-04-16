using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Rpg.Helpers;
using Rpg.Protocols;

namespace Rpg.Services
{
    public class WorldGrpcService : WorldProtocol.WorldProtocolBase
    {
        public WorldGrpcService(WorldService worldSvc, JwtService jwtSvc, IMapper mapper, IDistributedCache distCache)
        {
            _worldSvc = worldSvc;
            _jwtSvc = jwtSvc;
            _mapper = mapper;
            _distCache = distCache;
        }

        public async override Task<ConnectPlayerResponse> ConnectPlayer(ConnectPlayerRequest req, ServerCallContext ctx)
        {
            var httpCtx = ctx.GetHttpContext();
            var ctxPlayer = _worldSvc.ConnectPlayerAsync(httpCtx);
            
            return await Task.FromResult(new ConnectPlayerResponse()
            {
                Player = _mapper.Map<PlayerPacket>(ctxPlayer)
            });
        }


        public async override Task<ChangePlayerNameResponse> ChangePlayerName(ChangePlayerNameRequest req, ServerCallContext ctx)
        {
            var valName = ValidName(req.Name, maxLen: 8);
            var httpCtx = ctx.GetHttpContext();
            var ctxPlayer = _worldSvc.ChangePlayerNameAsync(httpCtx, valName);

            return await Task.FromResult(new ChangePlayerNameResponse()
            {
                Player = _mapper.Map<PlayerPacket>(ctxPlayer)
            });
        }

        private static string ValidName(string val, int maxLen) => ValidHelper.StringLength("NAME", val, maxLen: maxLen);

        private readonly WorldService _worldSvc;
        private readonly JwtService _jwtSvc;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distCache;
    }
}

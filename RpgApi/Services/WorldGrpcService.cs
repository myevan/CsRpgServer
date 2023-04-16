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
            var ses = ValidSession(ctx);
            var ctxPlayer = _worldSvc.ConnectPlayerAsync(ses);
            
            return await Task.FromResult(new ConnectPlayerResponse()
            {
                Player = _mapper.Map<PlayerPacket>(ctxPlayer)
            });
        }


        public async override Task<ChangePlayerNameResponse> ChangePlayerName(ChangePlayerNameRequest req, ServerCallContext ctx)
        {
            var ses = ValidSession(ctx);
            var valName = ValidName(req.Name, maxLen: 8);
            var ctxPlayer = _worldSvc.ChangePlayerNameAsync(ses, valName);

            return await Task.FromResult(new ChangePlayerNameResponse()
            {
                Player = _mapper.Map<PlayerPacket>(ctxPlayer)
            });
        }

        private Session ValidSession(ServerCallContext ctx)
        {
            var httpCtx = ctx.GetHttpContext();
            var sesKey = ValidHelper.String("CLAIM_SESSION_KEY", _jwtSvc.LoadClaimSessionKey(httpCtx));
            return Session.Load(_distCache, sesKey);
        }

        private static string ValidName(string val, int maxLen) => ValidHelper.StringLength("NAME", val, maxLen: maxLen);

        private readonly WorldService _worldSvc;
        private readonly JwtService _jwtSvc;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distCache;
    }
}

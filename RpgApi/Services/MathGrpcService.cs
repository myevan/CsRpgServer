using Grpc.Core;
using Rpg.Grpc;

namespace Rpg.Services
{
    public class MathGrpcService : SvcMath.SvcMathBase
    {
        public override Task<ResAdd> Add(ReqAdd req, ServerCallContext ctx)
        {
            var res = new ResAdd();
            res.Result = 0;
            return Task.FromResult(res);
        }
    }
}

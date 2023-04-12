using Grpc.Core;

namespace Rpg.Helpers
{
    public static class ValidHelper
    {
        public static void Condition(bool isCond, StatusCode statusCode, string detail)
        {
            if (!isCond)
            {
                throw new RpcException(new Status(statusCode, $"RPC_EXC_{detail}"));
            }
        }

        public static string String(string name, string val)
        {
            Condition(!string.IsNullOrEmpty(val), StatusCode.InvalidArgument, $"EMPTY_{name}");
            return val;
        }

        public static string StringLength(string name, string val, int maxLen)
        {
            String(name, val);
            Condition(val.Length <= maxLen, StatusCode.InvalidArgument, $"TOO_LONG_{name}");
            return val;
        }

        public static T Object<T>(string name, T? obj) where T : class
        {
            if (obj == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"RPC_EXC_NOT_FOUND_{name}"));
            }

            return obj;
        }

    }
}

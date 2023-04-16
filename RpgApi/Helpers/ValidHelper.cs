using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Rpg.Models;
using System.Xml.Linq;

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

        public static string String(string name, string? inStr)
        {
            if (inStr == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"RPC_EXC_NULL_{name}"));
            }
            return inStr;
        }
        public static int Id(string name, int inId)
        {
            if (inId <= 0)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"RPC_EXC_WRONG_{name}"));
            }
            return inId;
        }

        public static T Object<T>(string name, T? inObj) where T : class
        {
            if (inObj == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"RPC_EXC_NOT_FOUND_{name}"));
            }

            return inObj;
        }

        public static string StringLength(string name, string? inStr, int maxLen)
        {
            var valStr = String(name, inStr);
            Condition(valStr.Length <= maxLen, StatusCode.InvalidArgument, $"TOO_LONG_{name}");
            return valStr;
        }

        public static void PlayeGoldCost(Player player, int inGoldCost)
        {
            Condition(inGoldCost > 0, StatusCode.InvalidArgument, "MINUS_GOLD_COST");
            Condition(player.Gold >= inGoldCost, StatusCode.OutOfRange, "OVER_GOLD_COST");
        }

        public static void StatLevel(Stat stat, int inOldLv)
        {
            Condition(stat.Lv == inOldLv, StatusCode.InvalidArgument, "STAT_LV_MISMATCHED");
        }
    }
}

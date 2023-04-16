using Microsoft.Extensions.Caching.Distributed;
using Rpg.Models;
using System;

namespace Rpg
{
    public class Session
    {
        const string PREFIX = "ses";
        const string USER_GUID = "user.guid";
        const string PLAYER_ID = "player.id";

        public string Key { get; set; }
        
        public static Session Create(IDistributedCache distCache)
        {
            var newKey = Guid.NewGuid().ToString();
            return new Session(distCache, newKey);
        }

        public static Session Load(IDistributedCache distCache, string inKey)
        {
            return new Session(distCache, inKey);
        }

        private Session(IDistributedCache distCache, string inKey)
        {
            _distCache = distCache;
            Key = inKey;
        }

        public void SetUserGuid(string inGuid)
        {
            var key = $"{PREFIX}/{Key}/{USER_GUID}";
            _distCache.SetString(key, inGuid);
        }

        public string GetUserGuid()
        {
            var key = $"{PREFIX}/{Key}/{USER_GUID}";
            return GetStr(key);
        }

        public void SetPlayerId(int inId)
        {
            var key = $"{PREFIX}/{Key}/{PLAYER_ID}";
            _distCache.SetString(key, inId.ToString());
        }

        public int GetPlayerId()
        {
            var key = $"{PREFIX}/{Key}/{PLAYER_ID}";
            return GetInt(key);
        }

        public string GetStr(string key, string def = "")
        {
            var valStr = _distCache.GetString(key);
            if (valStr == null)
                return def;

            return valStr;
        }

        public int GetInt(string key, int def = 0)
        {
            var valStr = _distCache.GetString(key);
            if (valStr == null)
                return def;

            var valInt = int.Parse(valStr);
            return valInt;
        }

        public Dictionary<string, object> ToDict()
        {
            var ret = new Dictionary<string, object>();
            ret.Add("key", this.Key);
            ret.Add(USER_GUID, GetUserGuid());
            ret.Add(PLAYER_ID, GetPlayerId());
            return ret;
        }

        private readonly IDistributedCache _distCache;
    }
}

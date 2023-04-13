using AutoMapper;
using Rpg.DbContexts;
using Rpg.Helpers;
using Rpg.Models;

namespace Rpg.Services
{
    public class GameService
    {
        public GameService(UserDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        public async Task<Player> EnterWorld(string reqGuid)
        {
            ValidGuid(reqGuid);

            var ctxCount = ValidAccount(await _dbCtx.TouchAccountAsync(reqGuid, "SECRET", () => new Player()));
            var ctxPlayer = ctxCount.Player;
            return ctxPlayer;   
        }

        public async Task<Player> ChangePlayerName(int authPlayerId, string reqName)
        {
            ValidName(reqName, maxLen: 8);

            var ctxPlayer = ValidPlayer(await _dbCtx.FindPlayerAsync(authPlayerId));
            ctxPlayer.Name = reqName;
            _dbCtx.SaveChanges();

            return ctxPlayer;
        }

        private static string ValidGuid(string val) => ValidHelper.String("GUID", val);
        private static string ValidName(string val, int maxLen) => ValidHelper.StringLength("NAME", val, maxLen: maxLen);

        private static Account ValidAccount(Account? obj) => ValidHelper.Object<Account>("ACCOUNT", obj);

        private static Player ValidPlayer(Player? obj) => ValidHelper.Object<Player>("PLAYER", obj);

        private readonly UserDbContext _dbCtx;
    }
}

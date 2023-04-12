using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rpg.Configs;
using Rpg.Helpers;
using Rpg.Services;

namespace Rpg.Examples
{
    public class GameServiceExample
    {
        public static async void Run()
        {
            var dbOpts = new DbContextOptionsBuilder<UserDbContext>().UseInMemoryDatabase(databaseName: "UserDb").Options;
            var dbCtx = new UserDbContext(dbOpts);

            var jwtCfg = new JwtAuthConfig();
            var authSvc = new JwtAuthService(jwtCfg);
            var gameSvc = new GameService(dbCtx);

            var player = await gameSvc.EnterWorld("GUID");
            var player2 = gameSvc.ChangePlayerName(player.Id, "NEW_NAME");
        }
    }
}

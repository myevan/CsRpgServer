using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rpg.Configs;
using Rpg.Helpers;
using Rpg.Services;

namespace Rpg.Examples
{
    public class GrpServiceExample
    {
        public static void Run()
        {
            var dbOpts = new DbContextOptionsBuilder<UserDbContext>().UseInMemoryDatabase(databaseName: "UserDb").Options;
            var dbCtx = new UserDbContext(dbOpts);

            var jwtCfg = new JwtAuthConfig();
            var authSvc = new JwtAuthService(jwtCfg);
            var gameSvc = new GameService(dbCtx);

            var mapperCfg = new MapperConfiguration(cfg => {
                cfg.AddProfile<AutoMapperProfile>();
            });
            var mapper = new Mapper(mapperCfg);
            var grpcSvc = new GameGrpcService(gameSvc, authSvc, mapper);
        }
    }
}

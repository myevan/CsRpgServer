using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Rpg.DbContexts;
using Rpg.Helpers;
using Rpg.Models;
using Rpg.Protocols;

namespace Rpg.Services
{
    public class AuthService
    {
        public AuthService(AuthDbContext dbCtx, IDistributedCache distCache)
        {
            _dbCtx = dbCtx;
            _distCache = distCache;
        }

        public async Task<Session> SignUpAsync(string reqUserName)
        {
            var oldUser = await _dbCtx.FindUserAsync(reqUserName);
            ValidHelper.Condition(oldUser == null, StatusCode.InvalidArgument, "USER_ALREADY_CREATED");

            var newUser = ValidUser(await _dbCtx.CreateUserAsync(reqUserName));
            
            var newSession = Session.Create(_distCache);
            newSession.SetUserGuid(newUser.Guid);

            return newSession;
        }
        
        public async Task<Session> SignInAsync(string reqUserName, string reqPassword)
        {
            var oldUser = ValidUser(await _dbCtx.FindUserAsync(reqUserName));
            ValidHelper.Condition(oldUser.Password == reqPassword, StatusCode.InvalidArgument, "PASSWORD_MISMATCHED");
            
            var newSession = Session.Create(_distCache);
            newSession.SetUserGuid(oldUser.Guid);
            return newSession;
        }
        
        private User ValidUser(User? user)
        {
            return ValidHelper.Object<User>("USER", user);
        }

        private AuthDbContext _dbCtx;
        private IDistributedCache _distCache;
    }
}

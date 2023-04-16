using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Rpg.Protocols;
using Rpg.DbContexts;
using Rpg.Helpers;
using Rpg.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Rpg.Services
{
    public class AuthGrpcService : AuthProtocol.AuthProtocolBase
    {
        public AuthGrpcService(AuthService authSvc, JwtService jwtSvc)
        {
            _authSvc = authSvc;
            _jwtSvc = jwtSvc;
        }

        [AllowAnonymous]
        public async override Task<SignUpResponse> SignUp(SignUpRequest req, ServerCallContext ctx)
        {
            var ses = await _authSvc.SignUpAsync(req.UserName);
            var newToken = _jwtSvc.DumpSession(ses);
            return await Task.FromResult(new SignUpResponse() { Token = newToken });
        }

        [AllowAnonymous]
        public async override Task<SignInResponse> SignIn(SignInRequest req, ServerCallContext ctx)
        {
            var ses = await _authSvc.SignInAsync(req.UserName, req.Password);
            var newToken = _jwtSvc.DumpSession(ses);
            return await Task.FromResult(new SignInResponse() { Token = newToken });
        }

        private AuthService _authSvc;
        private JwtService _jwtSvc;
    }
}

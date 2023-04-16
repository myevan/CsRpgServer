using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Rpg.Configs;
using Rpg.Helpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace Rpg.Services
{
    public class JwtService
    {
        const string CLAIM_SESSION = "SESSION";

        public JwtService(JwtAuthConfig cfg, IDistributedCache distCache)
        {
            _cfg = cfg;
            _distCache = distCache;
        }

        public string DumpSession(Session ses)
        {
            var claims = new[]
            {
                new Claim(CLAIM_SESSION, ses.Key)
            };

            return DumpClaims(claims);
        }

        public string DumpRole(string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, role),
            };

            return DumpClaims(claims);
        }

        private string DumpClaims(Claim[] claims)
        {
            var token = new JwtSecurityToken
            (
                issuer: _cfg.Issuer,
                audience: _cfg.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg.Key)),
                    SecurityAlgorithms.HmacSha256)
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public Session? LoadSession(HttpContext ctx)
        {
            var sesKey = LoadClaimSessionKey(ctx);
            if (sesKey == null)
            {
                return null;
            }
            return Session.Load(_distCache, sesKey);
        }

        public string? LoadClaimSessionKey(HttpContext ctx)
        {
            var claimValue = LoadClaim(ctx, CLAIM_SESSION);
            return claimValue;
        }

        public string? LoadClaim(HttpContext ctx, string claimType)
        {
            var claimValue = ctx.User.FindFirstValue(claimType);
            return claimValue;
        }

        public string? GetEncryptedClaim(HttpContext ctx, string claimType)
        {            
            var claimValue = ctx.User.FindFirstValue(claimType);
            if (claimValue == null) return null;
            var decryptedValue = Decrypt(claimValue);
            return decryptedValue;
        }

        public string? GetEncryptedClaimWithCache(HttpContext ctx, string claimType, IMemoryCache cache)
        {
            var sessionKey = LoadClaimSessionKey(ctx);
            var cacheKey = $"{sessionKey}.{claimType}";
            if (cache.TryGetValue(cacheKey, out string? cacheValue))
            {
                return cacheValue;
            }

            var claimValue = GetEncryptedClaim(ctx, claimType);
            cache.Set(cacheKey, claimValue);
            return claimValue;
        }

        private string Encrypt(string src)
        {
            var enc = AesHelper.EncryptString(src, _cfg.AesKeyBytes, _cfg.AesIvBytes);
            return enc;
        }

        private string Decrypt(string enc)
        {
            var dst = AesHelper.EncryptString(enc, _cfg.AesKeyBytes, _cfg.AesIvBytes);
            return dst;
        }

        private readonly JwtAuthConfig _cfg;
        private readonly IDistributedCache _distCache;
    }
}

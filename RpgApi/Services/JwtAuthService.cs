using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Rpg.Models;
using Rpg.Configs;

namespace Rpg.Services
{
    public class JwtAuthService
    {
        public JwtAuthService(JwtAuthConfig cfg)
        {
            _cfg = cfg;
        }

        public string CreatePlayerToken(int inPlayerId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, inPlayerId.ToString()),
                new Claim(ClaimTypes.Role, "PLAYER"),
            };

            return CreateToken(claims);
        }
        public string CreateRoleToken(string role)
        {
            var claims = new[]
            {                
                new Claim(ClaimTypes.Role, role),
            };

            return CreateToken(claims);
        }

        private string CreateToken(Claim[] claims)
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

        public int GetPlayerId(HttpContext httpCtx)
        {
            var idStr = httpCtx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idStr == null) return 0;
            var id = int.Parse(idStr);
            return id;
        }

        private readonly JwtAuthConfig _cfg;
    }
}

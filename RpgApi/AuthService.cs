﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Rpg.Models;

namespace Rpg
{
    public class AuthService
    {
        public AuthService(string issuer, string audience, string keyStr)
        {
            _issuer = issuer;
            _audience = audience;
            _keyStr= keyStr;
        }

        public string CreateToken(int inPlayerId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "PLAYER"),
                new Claim(ClaimTypes.NameIdentifier, inPlayerId.ToString()),
            };

            var token = new JwtSecurityToken
            (
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_keyStr)),
                    SecurityAlgorithms.HmacSha256)
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public async Task<Player?> GetPlayer(HttpContext httpCtx, UserDbContext dbCtx)
        {
            var playerIdStr = httpCtx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (playerIdStr == null) return null;
            var playerId = int.Parse(playerIdStr);
            return await dbCtx.PlayerSet.FindAsync(playerId);
        }

        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _keyStr;
    }
}
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Epros_CareerHubAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Epros_CareerHubAPI.Helpers
{
    public interface IJwtHelper
    {
        string GenerateToken(Users user, string roleName);
        int GetExpiresMinutes();
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _config;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresMinutes;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
            _key = _config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key configuration missing");
            _issuer = _config["Jwt:Issuer"] ?? "eproscareerhub";
            _audience = _config["Jwt:Audience"] ?? "eproscareerhub";
            _expiresMinutes = int.TryParse(_config["Jwt:ExpiresMinutes"], out var m) ? m : 60;
        }

        public int GetExpiresMinutes() => _expiresMinutes;

        public string GenerateToken(Users user, string roleName)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));

            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // keep the 'sub' claim for standard compliance
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                // explicit name-identifier for easier server-side retrieval
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, roleName ?? string.Empty)
            };

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_expiresMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
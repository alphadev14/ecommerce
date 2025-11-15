using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace server.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateJwtToken(int userId, string userName, string roleName, string jwtId, IEnumerable<Claim>? extraClaims = null) 
        {
            var jwtKey = _config["Jwt:Key"] ?? "KeGbkhU3hIGXRELQga3XjfnT8EJci1KjISAF9UHGQmVYR9gdVzZWPHrjNDmeueQB";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "ecommerce";
            var jwtExpires = int.Parse(_config["Jwt:AccessTokenExpireMinutes"]);
            var jwtAudience = _config["Jwt:Audience"] ?? "ecommerce";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                 // Dùng để lấy UserId trong code
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),

                // Dùng để map vào HttpContext.User.Identity.Name
                new Claim(ClaimTypes.Name, userName),

                // Role (ASP.NET Core sẽ map đúng vào User.IsInRole)
                new Claim(ClaimTypes.Role, roleName),

                // Unique ID của token
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            };

            if (extraClaims != null)
            {
                claims.AddRange(extraClaims);
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtExpires),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

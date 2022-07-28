using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPI.Utilities;

namespace WebAPI.Services
{
    public class JwtAuthService : ITokenBasedAuthService<string>
    {
        private readonly IConfiguration _config;

        public JwtAuthService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateAccessToken(params Claim[] claims)
        {
            return CreateTokenWithClaimsAndExpiry(
                DateTime.UtcNow.AddMinutes(2), claims);
        }

        public string CreateRefreshToken(params Claim[] claims)
        {
            var rand = new Random();
            var elapsedTicks = DateTime.UtcNow.Ticks - new DateTime(2022, 1, 1).Ticks;
            var elapsedSpan = new TimeSpan(elapsedTicks);
            var randomValue = rand.Next((int)elapsedSpan.TotalMinutes, int.MaxValue);
            var newClaims = claims.Append(
                new Claim("randomString", randomValue.ToString())).ToArray();

            return CreateTokenWithClaimsAndExpiry(
                DateTime.UtcNow.AddMonths(3), newClaims);
        }

        private string CreateTokenWithClaimsAndExpiry(DateTime expiry, params Claim[] claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config[Statics.SecretConfigName]));
            var signature = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: expiry,
                claims: claims,
                signingCredentials: signature);

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }
    }
}

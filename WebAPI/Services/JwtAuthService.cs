using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebAPI.Services
{
    public class JwtAuthService : ITokenBasedAuthService<string>
    {
        private readonly IConfiguration _config;

        public JwtAuthService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(params Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSecret"]));
            var signature = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signature);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

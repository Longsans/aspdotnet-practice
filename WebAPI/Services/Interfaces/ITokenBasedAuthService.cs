using System.Security.Claims;

namespace WebAPI.Services
{
    public interface ITokenBasedAuthService<TTokenType>
    {
        TTokenType CreateAccessToken(params Claim[] claims);
        TTokenType CreateRefreshToken(params Claim[] claims);
    }
}

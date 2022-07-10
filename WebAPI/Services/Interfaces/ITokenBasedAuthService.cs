using System.Security.Claims;

namespace WebAPI.Services
{
    public interface ITokenBasedAuthService<TTokenType>
    {
        TTokenType CreateToken(params Claim[] claims);
    }
}

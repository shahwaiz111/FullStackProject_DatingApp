using MyDatingApp.Api.Entities;

namespace MyDatingApp.Api.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
        string GenerateRefreshToken();
    }
}

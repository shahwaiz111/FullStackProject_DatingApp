using MyDatingApp.Api.Entities;

namespace MyDatingApp.Api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}

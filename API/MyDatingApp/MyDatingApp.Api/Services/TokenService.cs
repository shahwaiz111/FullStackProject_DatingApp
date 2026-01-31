using Microsoft.IdentityModel.Tokens;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyDatingApp.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        public string CreateToken(AppUser user)
        {
            var tokenkey = _config["tokenkey"] ?? throw new Exception("cannot get token key");
            if (tokenkey.Length < 64)
            {
                throw new Exception("your token key needs to be greater than or equal to 64 characters");
            }
            // use our token key to create security key..
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenkey)); // This key will use to sign the token

            // claims about the user
            var claims = new List<Claim>
            {
                new (ClaimTypes.Email, user.Email),
                new (ClaimTypes.NameIdentifier, user.Id)
            };

            // use to sign the Token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MyDatingApp.Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await EmailExists (registerDto.Email))
            {
                return BadRequest("Email taken");
            }
            var hmac = new HMACSHA512();

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)

            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x=>x.Email == loginDto.Email); 
            if (user == null)
            {
                return Unauthorized("invalid email address");
            }
            var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("invalid password");
                }
            }
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)

            };

        }

        private async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(x=>x.Email.ToLower() == email.ToLower());

        }
    }
}

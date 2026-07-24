using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Interfaces;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyDatingApp.Api.Controllers
{
    public class AccountController : BaseApiController
    {
        //private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, ApplicationDbContext context)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            //if (await EmailExists (registerDto.Email))
            //{
            //    return BadRequest("Email taken");
            //}
            //var hmac = new HMACSHA512();

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                //PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                //PasswordSalt = hmac.Key,
                UserName = registerDto.Email,
                Member = new Member
                {
                    DisplayName = registerDto.DisplayName,
                    Gender = registerDto.Gender,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    DateOfBirth = registerDto.DateOfBirth
                }
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("identity", error.Description);
                }

                return ValidationProblem();
            }
            await _userManager.AddToRoleAsync(user, "Member");
            await SetRefreshTokenCookie(user);
            //await _context.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user)

            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //var user = await _context.Users.SingleOrDefaultAsync(x=>x.Email == loginDto.Email); 
            var user = await _context.Users.Include(x => x.Member).SingleOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email address");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Invalid password");
            await SetRefreshTokenCookie(user);
            //var hmac = new HMACSHA512(user.PasswordSalt);
            //var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //for (var i = 0; i < computedHash.Length; i++)
            //{
            //    if (computedHash[i] != user.PasswordHash[i])
            //    {
            //        return Unauthorized("invalid password");
            //    }
            //}
            //Console.WriteLine(user.Member == null);
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                ImageUrl = user.Member?.ImageUrl,
                Token = await _tokenService.CreateToken(user)

            };

        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null) return NoContent();

            var user = await _userManager.Users.Include(x => x.Member).FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null) return Unauthorized();
            await SetRefreshTokenCookie(user);
            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                ImageUrl = user.Member?.ImageUrl,
                Token = await _tokenService.CreateToken(user)

            };
        }

        private async Task SetRefreshTokenCookie(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }



        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await _userManager.Users.Where(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)).ExecuteUpdateAsync(setters => setters.SetProperty(x => x.RefreshToken, _ => null).SetProperty(x=>x.RefreshTokenExpiry, _=>null));
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        //private async Task<bool> EmailExists(string email)
        //{
        //    return await _context.Users.AnyAsync(x=>x.Email!.ToLower() == email.ToLower());

        //}
    }

}
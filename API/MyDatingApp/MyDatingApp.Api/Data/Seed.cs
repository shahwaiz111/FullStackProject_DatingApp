using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MyDatingApp.Api.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            // Seed Data into Database..

            //Console.WriteLine("seed method running..");
            
            if (await userManager.Users.AnyAsync()) return;
            var memberData = await File.ReadAllTextAsync("Data/UserDataSeed.json");
            
            // Deserialize into C# obj
            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);
            if (members == null)
            {
                Console.WriteLine("No members in seed data");
                return;
            }
            

            foreach ( var member in members )
            {
                //using var hmac = new HMACSHA512();
                var user = new AppUser
                {    
                    Id = member.Id,
                    Email = member.Email,
                    UserName = member.Email,
                    DisplayName = member.DisplayName,
                    ImageUrl = member.ImageUrl,
                    //PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                    //PasswordSalt = hmac.Key,

                    Member = new Member 
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        Description = member.Description,
                        DateOfBirth = member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Gender = member.Gender,
                        City = member.City,
                        Country = member.Country,
                        LastActive = member.LastActive,
                        Created = member.Created
                    }
                };

                if (!string.IsNullOrEmpty(member.ImageUrl))
                {
                    user.Member.Photos.Add(new Photo
                    {
                        Url = member.ImageUrl!,
                        MemberId = member.Id
                    });
                }
                var result = await userManager.CreateAsync(user, "Pa$$w0rd");

                if (!result.Succeeded)
                {
                    Console.WriteLine(result.Errors.First().Description);
                }
                await userManager.AddToRoleAsync(user, "Member");
                //context.Users.Add(user);
            }

            var admin = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                DisplayName = "Admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
            //await context.SaveChangesAsync();
        }
    }
}

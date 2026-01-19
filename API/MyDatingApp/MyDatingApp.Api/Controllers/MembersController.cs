using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.Entities;

namespace MyDatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getallmembers")]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await _context.Users.ToListAsync();
            return members;
        }

        [HttpGet("getmemberbyid/{id}")]
        public async Task<ActionResult<AppUser>> GetMemberById(string id)
        {
            var member = await _context.Users.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return member;
        }
    }
}

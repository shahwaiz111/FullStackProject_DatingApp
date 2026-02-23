using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Interfaces;

namespace MyDatingApp.Api.Controllers
{
    [Authorize]
    public class MembersController : BaseApiController
    {
        private readonly IMemberRepository _memberRepository;
        public MembersController(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        
        [HttpGet("getallmembers")]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
        {
            return Ok(await _memberRepository.GetMemberAsync());
        }

        [HttpGet("getmemberbyid/{id}")]
        public async Task<ActionResult<Member>> GetMemberById(string id)
        {
            var member = await _memberRepository.GetMemberByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            return member;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return Ok(await _memberRepository.GetPhotosForMemberAsync(id));
        }
    }
}

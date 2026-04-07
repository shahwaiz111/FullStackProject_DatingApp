using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Interfaces;
using System.Security.Claims;

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

        [HttpPut("updatemember")]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            // id comes from Jwt Token..
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (memberId == null) return BadRequest("Oops - no id found in token..");

            var member = await _memberRepository.GetMemberByIdAsync(memberId);
            if (member == null) return BadRequest("could not get member");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            _memberRepository.Update(member);
            if (await _memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update member");
        }
    }
}

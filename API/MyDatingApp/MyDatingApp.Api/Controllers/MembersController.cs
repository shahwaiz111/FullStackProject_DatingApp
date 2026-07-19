using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Helpers;
using MyDatingApp.Api.Interfaces;
using MyDatingApp.Api.Services;
using System.Security.Claims;

namespace MyDatingApp.Api.Controllers
{
    [Authorize]
    public class MembersController : BaseApiController
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IPhotoService _photoService;
        public MembersController(IMemberRepository memberRepository, IPhotoService photoService)
        {
            _memberRepository = memberRepository;
            _photoService = photoService;
        }

        
        [HttpGet("getallmembers")]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery] MemberParams memberParams)
        {
            memberParams.CurrentMemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _memberRepository.GetMemberAsync(memberParams));
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

            var member = await _memberRepository.GetMemberForUpdate(memberId);
            if (member == null) return BadRequest("could not get member");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

            _memberRepository.Update(member);
            if (await _memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update member");
        }


        [HttpPost("add-photo")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Photo>> AddPhoto(IFormFile file)
        {
            var member = await _memberRepository.GetMemberForUpdate(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (member == null) return BadRequest("cannot update member");

            var result = await _photoService.UploadPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }
            member.Photos.Add(photo);

            if (await _memberRepository.SaveAllAsync()) return photo;
            return BadRequest("problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await _memberRepository.GetMemberForUpdate(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (member == null) return BadRequest("cannot get member from token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
            if (member.ImageUrl == photo?.Url || photo == null)
            {
                return BadRequest("cannot set this as main image..");
            }
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;

            if (await _memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await _memberRepository.GetMemberForUpdate(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (member == null) return BadRequest("cannot get member from token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);
            if (photo == null || photo.Url == member.ImageUrl)
            {
                return BadRequest("This photo cannot be deleted..");
            }
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message); 
            }
            
            member.Photos.Remove(photo);
            if (await _memberRepository.SaveAllAsync()) return Ok();
            return BadRequest("problem deleting the photo");
        }
    }
    
    }

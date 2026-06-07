using Microsoft.AspNetCore.Mvc;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Helpers;
using MyDatingApp.Api.Interfaces;
using System.Security.Claims;

namespace MyDatingApp.Api.Controllers
{
    public class LikesController(ILikesRepository likesRepository) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetMemberId)
        {
            var sourceMemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sourceMemberId == targetMemberId) return BadRequest("you cannot like yourself..");

            var existingLike = await likesRepository.GetMemberLike(sourceMemberId, targetMemberId);

            if (existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId
                };

                likesRepository.AddLike(like);
            }
            else
            {
                likesRepository.DeleteLike(existingLike);
            }
            if (await likesRepository.SaveAllChanges()) return Ok();

            return BadRequest("Failed to update like.."); 
        }

        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
        {
            return Ok(await likesRepository.GetCurrentMemberLikeIds(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes([FromQuery] LikeParams likeParams)
        {
            likeParams.MemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var members = await likesRepository.GetMemberLikes(likeParams);

            return Ok(members);
        }
    }
}

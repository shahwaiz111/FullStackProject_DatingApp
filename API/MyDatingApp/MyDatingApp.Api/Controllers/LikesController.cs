using Microsoft.AspNetCore.Mvc;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Helpers;
using MyDatingApp.Api.Interfaces;
using System.Security.Claims;

namespace MyDatingApp.Api.Controllers
{
    public class LikesController(IUnitOfWork uow) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetMemberId)
        {
            var sourceMemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sourceMemberId == targetMemberId) return BadRequest("you cannot like yourself..");

            var existingLike = await uow.LikesRepository.GetMemberLike(sourceMemberId, targetMemberId);

            if (existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId
                };

                uow.LikesRepository.AddLike(like);
            }
            else
            {
                uow.LikesRepository.DeleteLike(existingLike);
            }
            if (await uow.Complete()) return Ok();

            return BadRequest("Failed to update like.."); 
        }

        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikeIds()
        {
            return Ok(await uow.LikesRepository.GetCurrentMemberLikeIds(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes([FromQuery] LikeParams likeParams)
        {
            likeParams.MemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var members = await uow.LikesRepository.GetMemberLikes(likeParams);

            return Ok(members);
        }
    }
}

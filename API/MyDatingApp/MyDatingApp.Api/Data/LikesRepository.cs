using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Helpers;
using MyDatingApp.Api.Interfaces;

namespace MyDatingApp.Api.Data
{
    public class LikesRepository(ApplicationDbContext _context) : ILikesRepository
    {
        public void AddLike(MemberLike like)
        {
            _context.Likes.Add(like);        
        }

        public void DeleteLike(MemberLike like)
        {
            _context.Likes.Remove(like);
        }

        // who have i liked..
        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
        {
            // so we're going to check for a list of likes that the current user has in the likes table..
            return await _context.Likes.Where(x=>x.SourceMemberId == memberId).Select(x=>x.TargetMemberId).ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
        {
            return await _context.Likes.FindAsync(sourceMemberId, targetMemberId);
        }

        public async Task<PaginatedResult<Member>> GetMemberLikes(LikeParams likeParams)
        {
            // building query gradually cant execute yet..
            var query = _context.Likes.AsQueryable();
            IQueryable<Member> result;

            switch (likeParams.Predicate)
            {
                case "liked":
                    result = query.Where(like => like.SourceMemberId == likeParams.MemberId).Select(like => like.TargetMember);
                    break;

                case "likedby":
                    result = query.Where(like => like.TargetMemberId == likeParams.MemberId).Select(like => like.SourceMember);
                    break;

                default:
                    var likeIds = await GetCurrentMemberLikeIds(likeParams.MemberId);

                    result = query.Where(x => x.TargetMemberId == likeParams.MemberId && likeIds.Contains(x.SourceMemberId))
                        .Select(x => x.SourceMember);
                    break;
            }
            return await PaginationHelper.CreateAsync(result, likeParams.PageNumber, likeParams.PageSize);
                    
                    
            }

        //public async Task<bool> SaveAllChanges()
        //{
        //    return await _context.SaveChangesAsync() > 0; 
        //}
    }
}

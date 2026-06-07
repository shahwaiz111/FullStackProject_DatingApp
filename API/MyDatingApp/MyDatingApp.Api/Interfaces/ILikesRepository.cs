using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Helpers;

namespace MyDatingApp.Api.Interfaces
{
    public interface ILikesRepository
    {
        // prevent duplicate likes..
        Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);

        Task<PaginatedResult<Member>> GetMemberLikes(LikeParams likeParams);
        // members that current member has liked..
        Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
        void DeleteLike(MemberLike like);
        void AddLike(MemberLike like);
        Task<bool> SaveAllChanges();

    }
}

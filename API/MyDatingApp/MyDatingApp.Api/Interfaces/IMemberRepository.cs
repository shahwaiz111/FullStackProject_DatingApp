using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Helpers;

namespace MyDatingApp.Api.Interfaces
{
    public interface IMemberRepository
    {
        void Update(Member member);
        //Task<bool> SaveAllAsync();
        Task<PaginatedResult<Member>> GetMemberAsync(MemberParams memberParams);
        Task<Member> GetMemberByIdAsync(string id);
        Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId);
        Task<Member?> GetMemberForUpdate(string id);
    }
}

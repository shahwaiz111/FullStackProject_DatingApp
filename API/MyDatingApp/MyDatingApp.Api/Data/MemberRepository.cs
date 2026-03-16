 using Microsoft.EntityFrameworkCore;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Interfaces;
using System.Threading.Tasks;

namespace MyDatingApp.Api.Data
{
    public class MemberRepository(ApplicationDbContext context) : IMemberRepository
    {
        public async Task<IReadOnlyList<Member>> GetMemberAsync()
        {
            return await context.Members.ToListAsync();
        }

        public async Task<Member> GetMemberByIdAsync(string id)
        {
            var member = await context.Members.FindAsync(id);
            if (member == null)
            {
                return null;
            }
            return member;
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
        {
            return await context.Members.Where(x => x.Id == memberId).SelectMany(x=>x.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(Member member)
        {
            context.Entry(member).State = EntityState.Modified;

        }


    }
}

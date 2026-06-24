using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyDatingApp.Api.Entities
{
    public class Member
    {
        public string Id { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public required string DisplayName { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public required string Gender { get; set; }
        public string? Description { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }

        // Navigation property '1 user is related to 1 member'..

        [JsonIgnore]
        [ForeignKey(nameof(Id))]    
        public AppUser User { get; set; } = null!;

        [JsonIgnore]
        public List<Photo> Photos { get; set; } = [];

        // that is going to represent a list of users that are liked by other users..
        [JsonIgnore]
        public List<MemberLike> LikedByMembers { get; set; } = [];

        // so it is going to be the list of users the current user likes.. 
        [JsonIgnore]
        public List<MemberLike> LikedMembers { get; set; } = [];

        [JsonIgnore]
        public List<Message> MessagesSent { get; set; } = [];
        [JsonIgnore]
        public List<Message> MessagesReceived { get; set; } = [];

    }
}

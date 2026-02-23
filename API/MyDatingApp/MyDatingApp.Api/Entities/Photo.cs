using System.Text.Json.Serialization;

namespace MyDatingApp.Api.Entities
{
    public class Photo
    {
        public int Id { get; set; } 
        public required string Url { get; set; }
        public string? PublicId { get; set; }

        // Nav Property
        [JsonIgnore]
        public Member Member { get; set; } = null!;
        public string MemberId { get; set; } = null!;   
    }
}

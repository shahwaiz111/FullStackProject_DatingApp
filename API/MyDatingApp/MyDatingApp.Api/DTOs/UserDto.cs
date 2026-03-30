namespace MyDatingApp.Api.DTOs
{
    public class UserDto
    {
        public required string Id { get; set; }
        //public required string Gender { get; set; }
        //public required string City { get; set; }
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public string? ImageUrl { get; set; }
        public required string Token { get; set; }
    }
}

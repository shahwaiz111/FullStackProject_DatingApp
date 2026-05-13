using System.ComponentModel.DataAnnotations;

namespace MyDatingApp.Api.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [MinLength(4)]
        public string Password { get; set; } = "";

        public string Gender { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public DateOnly DateOfBirth { get; set; } 
    }
}

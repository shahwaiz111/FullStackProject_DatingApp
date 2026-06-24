namespace MyDatingApp.Api.DTOs
{
    public class CreateMessageDto
    {
        public required string RecipientID { get; set; }
        public required string Content { get; set; }
    }
}

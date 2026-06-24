using Microsoft.AspNetCore.Mvc;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Extensions;
using MyDatingApp.Api.Helpers;
using MyDatingApp.Api.Interfaces;
using System.Security.Claims;

namespace MyDatingApp.Api.Controllers
{
    public class MessagesController(IMessageRepository messageRepository, IMemberRepository memberRepository) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var sender = await memberRepository.GetMemberByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientID);
            if (recipient == null || sender.Id == null || sender.Id == createMessageDto.RecipientID) 
                return BadRequest("cannot send this message");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content
            };
            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) return message.ToDto();
            return BadRequest("failed to send message"); 
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer([FromQuery] MessageParams messageParams)
        {
            messageParams.MemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await messageRepository.GetMessagesForMember(messageParams);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
        {
            return Ok(await messageRepository.GetMessageThread(User.FindFirstValue(ClaimTypes.NameIdentifier), recipientId));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(string id)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var message = await messageRepository.GetMessage(id);

            if (message == null) return BadRequest("cannot delete this message");

            if (message.SenderId != memberId && message.RecipientId != memberId) return BadRequest("you cannot delete this message");
            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            if (message is {SenderDeleted:true, RecipientDeleted: true })
            {
                messageRepository.DeleteMessage(message);
            }
            if (await messageRepository.SaveAllAsync()) return Ok();
            return BadRequest("problem deleting the message"); 
        }
    }

    

}

using Microsoft.AspNetCore.Mvc;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Extensions;
using MyDatingApp.Api.Helpers;
using MyDatingApp.Api.Interfaces;
using System.Security.Claims;

namespace MyDatingApp.Api.Controllers
{
    public class MessagesController(IUnitOfWork uow) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
                var sender = await uow.MemberRepository.GetMemberByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var recipient = await uow.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientID);
            if (recipient == null || sender.Id == null || sender.Id == createMessageDto.RecipientID) 
                return BadRequest("cannot send this message");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content
            };
            uow.MessageRepository.AddMessage(message);

            if (await uow.Complete()) return message.ToDto();
            return BadRequest("failed to send message"); 
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessagesByContainer([FromQuery] MessageParams messageParams)
        {
            messageParams.MemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await uow.MessageRepository.GetMessagesForMember(messageParams);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
        {
            return Ok(await uow.MessageRepository.GetMessageThread(User.FindFirstValue(ClaimTypes.NameIdentifier), recipientId));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(string id)
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var message = await uow.MessageRepository.GetMessage(id);

            if (message == null) return BadRequest("cannot delete this message");

            if (message.SenderId != memberId && message.RecipientId != memberId) return BadRequest("you cannot delete this message");
            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            if (message is {SenderDeleted:true, RecipientDeleted: true })
            {
                uow.MessageRepository.DeleteMessage(message);
            }
            if (await uow.Complete()) return Ok();
            return BadRequest("problem deleting the message"); 
        }
    }

    

}

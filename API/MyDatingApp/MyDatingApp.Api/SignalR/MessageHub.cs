using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using MyDatingApp.Api.Data;
using MyDatingApp.Api.DTOs;
using MyDatingApp.Api.Entities;
using MyDatingApp.Api.Extensions;
using MyDatingApp.Api.Interfaces;
using System.Security.Claims;

namespace MyDatingApp.Api.SignalR;

    [Authorize]
    public class MessageHub(IUnitOfWork uow, IHubContext<PresenceHub> presenceHub ) : Hub
    {
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new HubException("Other user not found");
        var groupName = GetGroupName(GetUserId(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var messages = await uow.MessageRepository.GetMessageThread(GetUserId(), otherUser);
        await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        {
            var sender = await uow.MemberRepository.GetMemberByIdAsync(GetUserId());
            var recipient = await uow.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientID);
            if (recipient == null || sender.Id == null || sender.Id == createMessageDto.RecipientID)
                throw new HubException("cannot send message");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.Id, recipient.Id);
            var group = await uow.MessageRepository.GetMessageGroup(groupName);
            var userInGroup = group != null && group.Connections.Any(x => x.UserId == recipient.Id);

            if (userInGroup)
            {
                message.DateRead = DateTime.UtcNow; 
            }
            uow.MessageRepository.AddMessage(message);

            if (await uow.Complete())
            {
                //var group = GetGroupName(sender.Id, recipient.Id);
                await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id);
                if (connections != null && connections.Count > 0 && !userInGroup)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToDto());
                }
            }
            
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await uow.MessageRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await uow.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, GetUserId());

        if (group == null)
        {
            group = new Group(groupName);
            uow.MessageRepository.AddGroup(group);
        }
        group.Connections.Add(connection);
        return await uow.Complete();
    }

    private string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";

    }
    private string GetUserId()
    {
        return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("cannot get member  id");
    }
}


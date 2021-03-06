using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Controllers.Resources;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Persistence;

namespace Upico.Controllers.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UpicODbContext _context; // it's a temporary solution.
        public ChatHub(IUnitOfWork unitOfWork, IMapper mapper, UpicODbContext context)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._context = context;
        }

        public async Task SendMessage(SendMessageResource sendMessageResource)
        {
            var receiverId = sendMessageResource.ReceiverId;
            var message = sendMessageResource.Message;

            var context = Context.GetHttpContext();
            var userName = context.User.Identity.Name;

            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user != null)
            {
                var messageHub = await this._unitOfWork.MessageHubs.GetMessageHub(user.Id, receiverId);
                if (messageHub != null)
                {
                    var newMessage = new Message();
                    newMessage.Content = message;
                    messageHub.Messages.Add(newMessage);

                    await this._unitOfWork.Complete();

                    var receiverMessageHub = await this._unitOfWork.MessageHubs.GetMessageHub(receiverId, user.Id);
                    var messageResource = this._mapper.Map<MessageResource>(newMessage);


                    await Clients.Caller.SendAsync("ReceiveMessage", messageResource);
                    messageResource.MessageHubId = receiverMessageHub.Id;
                    await Clients.Group(receiverId).SendAsync("ReceiveMessage", messageResource);

                }

            }
        }
        public async Task WithdrawnMessage(string messageId)
        {
            var context = Context.GetHttpContext();
            var userName = context.User.Identity.Name;

            var message = await this._context.Messages.SingleOrDefaultAsync(m => m.Id == new Guid(messageId));
            if (message == null)
                return ;

            var messageHub = await this._context.MessageHubs.SingleOrDefaultAsync(m => m.Id == message.MessageHubId);

            message.IsWithDraw = true;

            await this._context.SaveChangesAsync();

            var messageResource = this._mapper.Map<MessageResource>(message);
            messageResource.Content = null;
            await Clients.Group(messageHub.SenderId).SendAsync("WithdrawnMessage", messageResource);
            await Clients.Group(messageHub.ReceiverId).SendAsync("WithdrawnMessage", messageResource);
        }
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            var userName = context.User.Identity.Name;

            var user = await this._unitOfWork.Users.GetUser(userName);
            if(user != null)
            {
                var senderMessageHubs = await this._unitOfWork.MessageHubs.GetMessageHubs(user.Id);

                var senderMessageHubResources = this._mapper.Map<List<MessageHubResource>>(senderMessageHubs);
                foreach (var senderMessageHubResource in senderMessageHubResources)
                {
                    var receiverMessageHub = await this._unitOfWork.MessageHubs
                        .GetMessageHub(senderMessageHubResource.ReceiverId, senderMessageHubResource.SenderId);

                    var receiverMessageHubResource = this._mapper.Map<MessageHubResource>(receiverMessageHub);


                    var concatMessageResources = senderMessageHubResource.Messages.Concat(receiverMessageHubResource.Messages).OrderBy(m => m.CreatedAt).ToList();

                    foreach (var messageResource in concatMessageResources)
                    {
                        if (messageResource.IsWithDraw)
                        {
                            messageResource.Content = null;
                        }
                    }
                    senderMessageHubResource.Messages = concatMessageResources;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, user.Id);
            
                await Clients.Caller.SendAsync("LoadMessages", senderMessageHubResources);
            }

        }
    }
}

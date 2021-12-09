using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.Repositories;

namespace Upico.Persistence.Repositories
{
    public class MessageHubRepository : Repository<MessageHub>, IMessageHubRepository
    {
        private readonly UpicODbContext _context;

        public MessageHubRepository(UpicODbContext context)
            : base(context)
        {
            this._context = context;
        }

        public async Task<List<MessageHub>> GetMessageHubs(string userId)
        {
            var messageHubs = await this._context.MessageHubs
                .Include(mh => mh.Receiver)
                .Include(mh => mh.Messages)
                .Where(mh => mh.SenderId == userId)
                .ToListAsync();
            await this._context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            return messageHubs;

        }

        public async Task<MessageHub> GetMessageHub (string senderId, string receiverId)
        {
            var messageHub = await this._context.MessageHubs.Include(mh => mh.Messages).SingleOrDefaultAsync(mh => mh.SenderId == senderId && mh.ReceiverId == receiverId);
            await this._context.Users.SingleOrDefaultAsync(u => u.Id == senderId);
            await this._context.Users.SingleOrDefaultAsync(u => u.Id == receiverId);

            return messageHub;

        }

        public async Task<MessageHub> CreateMessageHub(string firstUserId, string secondUserId)
        {
            var firstUser = await this._context.Users.SingleOrDefaultAsync(u => u.Id == firstUserId);
            var secondUser = await this._context.Users.SingleOrDefaultAsync(u => u.Id == secondUserId);

            var messageHub = new MessageHub();
            messageHub.SenderId = firstUserId;
            messageHub.ReceiverId = secondUserId;

            await this._context.MessageHubs.AddAsync(messageHub);

            return messageHub;
        }
    }
}

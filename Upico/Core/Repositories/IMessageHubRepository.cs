using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;

namespace Upico.Core.Repositories
{
    public interface IMessageHubRepository : IRepository<MessageHub>
    {
        public Task<List<MessageHub>> GetMessageHubs(string userId);
        public Task<MessageHub> CreateMessageHub(string firstUserId, string secondUserId);
        public Task<MessageHub> GetMessageHub(string senderId, string receiverId);
    }


}

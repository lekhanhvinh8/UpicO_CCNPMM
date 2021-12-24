using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class MessageHubResource
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverUserName { get; set; }
        public string ReceiverAvatarUrl { get; set; }
        public DateTime ReceiverLastAccessed { get; set; }
        public IList<MessageResource> Messages { get; set; }
        public MessageHubResource()
        {
            this.Messages = new List<MessageResource>();
        }
    }
}

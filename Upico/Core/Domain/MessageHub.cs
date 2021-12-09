using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.Domain
{
    public class MessageHub
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public AppUser Sender { get; set; }
        public AppUser Receiver { get; set; }
        public IList<Message> Messages { get; set; }
        public MessageHub()
        {
            this.Messages = new List<Message>();
        }

    }
}

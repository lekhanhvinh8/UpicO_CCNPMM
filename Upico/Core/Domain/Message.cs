using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.Domain
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid MessageHubId { get; set; }
        public MessageHub MessageHub { get; set; }
        public bool IsWithDraw { get; set; }
    }
}

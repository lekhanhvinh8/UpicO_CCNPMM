using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class MessageResource
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid MessageHubId { get; set; }
        public string SenderUserName { get; set; }
        public string SenderId { get; set; }
        public bool IsWithDraw { get; set; }


    }
}

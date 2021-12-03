using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Controllers.Resources;

namespace Upico.Controllers.SignalR
{
    public class ChatHub : Hub
    {
        public ChatHub()
        {

        }

        public async Task SendComment(CommentResouce comment)
        {
            await Clients.Group("abc").SendAsync("receivedComment", comment);
        }
    }
}

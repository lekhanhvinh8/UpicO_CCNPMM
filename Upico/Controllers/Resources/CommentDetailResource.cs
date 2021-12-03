using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class CommentDetailResource
    {
        public Guid Id { set; get; }
        public string Username { set; get; }
        public string UserDisplayName { get; set; }
        public string UserAvatarUrl { get; set; }
        public string Content { set; get; }
        public DateTime DateCreate { set; get; }
        public int Replies { get; set; }
    }
}

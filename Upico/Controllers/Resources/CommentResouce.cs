using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class CommentResouce
    {
        public Guid Id { set; get; }
        public string Username { set; get; }
        public string UserDisplayName { get; set; }
        public string UserAvatarUrl { get; set; }
        public string Content { set; get; }
        public DateTime DateCreate { set; get; }
        public List<CommentResouce> Childs { get; set; }
        public CommentResouce()
        {
            this.Childs = new List<CommentResouce>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.Domain
{
    public class Comment
    {
        public Guid Id { set; get; }
        public string UserId { set; get; }
        public Guid? PostId { set; get; }
        public Guid? ParentId { set; get; }
        public string Content { set; get; }
        public DateTime DateCreate { set; get; }

        public AppUser User { set; get; }
        public Post Post { set; get; }
        public Comment Parent { set; get; }
        public IList<Comment> Childs { get; set; }
        public Comment()
        {
            this.Childs = new List<Comment>();
        }

    }
}

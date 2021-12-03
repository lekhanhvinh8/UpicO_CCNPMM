using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class PostResouce
    {
        public Guid Id { set; get; }
        public string UserId { set; get; }
        public string Content { set; get; }
        public DateTime DateCreate { set; get; }
        public bool PrivateMode { get; set; }

    }
}

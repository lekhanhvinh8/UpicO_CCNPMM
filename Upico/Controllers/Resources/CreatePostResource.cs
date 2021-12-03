using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class CreatePostResource
    {
        public string UserName { set; get; }
        public string Content { set; get; }
        public bool PrivateMode { get; set; }
    }
}

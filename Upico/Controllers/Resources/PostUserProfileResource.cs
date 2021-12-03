using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Controllers.Resouces;

namespace Upico.Controllers.Resources
{
    public class PostUserProfileResource
    {
        public Guid Id { get; set; }
        public IList<PhotoResource> PostImages{ get; set; }
        public int Comments { get; set; }
        public int Likes { get; set; }
        public string Content { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.Domain
{
    public class PostedImage
    {
        public string Id { set; get; }
        public Guid PostId { set; get; }
        public string Path { set; get; }
        public Post Post { set; get; }
    }
}

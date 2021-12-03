using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class AvatarResource
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public DateTime UploadTime { get; set; }
        public int IsMain { get; set; }
    }
}

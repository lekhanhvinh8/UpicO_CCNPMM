using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.Domain
{
    public class Avatar
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public DateTime UploadTime { get; set; }
        public bool IsMain { get; set; }

        public string UserID { get; set; }
        public AppUser AppUser { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.Domain
{
    public class AccessLog
    {
        public int Id { get; set; }
        public DateTime LogTime { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}

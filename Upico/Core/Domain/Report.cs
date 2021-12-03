using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Core.Domain
{
    public class Report
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public AppUser Reporter { get; set; }
        public string ReporterId { get; set; }
        public string ReportContent { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

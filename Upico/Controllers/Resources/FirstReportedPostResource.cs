using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class FirstReportedPostResource
    {
        public Guid Id { get; set; }
        public int NumOfReports { get; set; }
        public DateTime FirstReportTime { get; set; }
        public string FirtsReporter { get; set; }
    }
}

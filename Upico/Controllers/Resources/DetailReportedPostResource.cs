using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Controllers.Resouces;

namespace Upico.Controllers.Resources
{
    public class DetailReportedPostResource
    {
        public Guid Id { set; get; }
        public string UserName { set; get; }
        public string Content { set; get; }
        public DateTime DateCreate { set; get; }
        public IList<PhotoResource> PostImages { set; get; }
        public IList<ReportResource> Reports { get; set; }
        public DetailReportedPostResource()
        {
            PostImages = new List<PhotoResource>();
            Reports = new List<ReportResource>();
        }
        
    }
}

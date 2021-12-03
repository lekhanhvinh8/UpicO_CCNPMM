using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.Repositories;

namespace Upico.Persistence.Repositories
{
    public class ReportedPostRepository : Repository<Report>, IReportedPostRepository
    {
        private readonly UpicODbContext _context;

        public ReportedPostRepository(UpicODbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}

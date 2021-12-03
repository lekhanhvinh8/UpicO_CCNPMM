using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.Repositories;

namespace Upico.Persistence.Repositories
{
    public class PostedImageRepository : Repository<PostedImage>, IPostedImageRepository
    {
        private readonly UpicODbContext _context;

        public PostedImageRepository(UpicODbContext context)
            :base(context)
        {
            _context = context;
        }
    }
}

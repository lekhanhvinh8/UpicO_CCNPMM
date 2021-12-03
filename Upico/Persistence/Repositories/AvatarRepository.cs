using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.Repositories;

namespace Upico.Persistence.Repositories
{
    public class AvatarRepository : Repository<Avatar>, IAvatarRepository
    {
        private readonly UpicODbContext _context;

        public AvatarRepository(UpicODbContext context)
            :base(context)
        {
            this._context = context;
        }
        public async Task<List<Avatar>> GetAvatars(string userName)
        {
            var avatars = await this._context.Avatars.Where(a => a.AppUser.UserName == userName).ToListAsync();

            return avatars;
        }
    }
}

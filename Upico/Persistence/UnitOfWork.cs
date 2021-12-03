using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core;
using Upico.Core.Repositories;
using Upico.Persistence.Repositories;

namespace Upico.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UpicODbContext _context;

        public UnitOfWork(UpicODbContext context, 
            IAvatarRepository avatarRepository,
            IUserRepository userRepository,
            IPostRepository postRepository,
            IPostedImageRepository postedImageRepository,
            ICommentRepository commentRepository,
            IReportedPostRepository reportedPostRepository)
        {
            //a data context that be injected in runtime is the same for both UnitOfWork and CustomRepository

            //So when we modify data in CustomRepository and save it in UnitOfWork.Complete, 2 contexts that are working
            //in order to modify and save data is the same context

            this._context = context;

            Avatars = avatarRepository;
            Users = userRepository;
            Posts = postRepository;
            PostedImages = postedImageRepository;
            Comments = commentRepository;
            ReportedPosts = reportedPostRepository;
        }

        public IAvatarRepository Avatars { get; private set; }
        public IUserRepository Users { get; private set; }
        public IPostRepository Posts { get; private set; }
        public IPostedImageRepository PostedImages { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public IReportedPostRepository ReportedPosts { get; set; }
        public async Task<int> Complete()
        {
            return await this._context.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._context.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Repositories;

namespace Upico.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IAvatarRepository Avatars { get; }
        IUserRepository Users { get; }
        IPostRepository Posts { get; }
        IPostedImageRepository PostedImages { get; }
        ICommentRepository Comments { get; }
        IReportedPostRepository ReportedPosts { get; }
        Task<int> Complete();
    }
}

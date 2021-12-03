using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;

namespace Upico.Core.Services
{
    public interface ICommentService
    {
        public Task<IList<Comment>> GetParentComments(string postId, string lastCommentId, int numComments);
        public Task<IList<Comment>> GetChildrenComments(string parentId, string lastCommentId, int numComments);



    }
}

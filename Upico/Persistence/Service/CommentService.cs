using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Core.Services;

namespace Upico.Persistence.Service
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<IList<Comment>> GetParentComments(string postId, string lastCommentId, int numComments)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == postId);

            Comment lastComment = null;

            if (lastCommentId != null)
            {
                lastComment = await this._unitOfWork.Comments.SingleOrDefault(c => c.Id.ToString() == lastCommentId);
                await this._unitOfWork.Comments.Load(c => c.ParentId == lastComment.Id);
            }

            await this._unitOfWork.Comments.Load(c => c.PostId == post.Id);
            foreach (var comment in post.Comments)
            {
                await this._unitOfWork.Users.Load(u => u.Id == comment.UserId);
                await this._unitOfWork.Users.LoadMainAvatar(comment.User.UserName);

                await this._unitOfWork.Comments.Load(c => c.ParentId == comment.Id);
            }

            var comments = post.Comments.ToList();

            comments = comments.OrderByDescending(c => c, new ComparerComment(_unitOfWork)).ToList();

            if(lastComment != null)
                comments = comments.Where(c => new ComparerComment(_unitOfWork).Compare(c, lastComment) < 0).Take(numComments).ToList();
            else
                comments = comments.Take(numComments).ToList();

            return comments;
        }
        

        public async Task<IList<Comment>> GetChildrenComments(string parentId, string lastCommentId, int numComments)
        {
            var rootComment = await this._unitOfWork.Comments.SingleOrDefault(c => c.Id.ToString() == parentId);

            Comment lastComment = null;

            if (lastCommentId != null)
            {
                lastComment = await this._unitOfWork.Comments.SingleOrDefault(c => c.Id.ToString() == lastCommentId);
                await this._unitOfWork.Comments.Load(c => c.ParentId == lastComment.Id);
            }

            await this._unitOfWork.Comments.Load(c => c.ParentId == rootComment.Id);
            foreach (var comment in rootComment.Childs)
            {
                await this._unitOfWork.Users.Load(u => u.Id == comment.UserId);
                await this._unitOfWork.Users.LoadMainAvatar(comment.User.UserName);

                await this._unitOfWork.Comments.Load(c => c.ParentId == comment.Id);
            }

            var comments = rootComment.Childs.ToList();

            comments = comments.OrderBy(c => c.DateCreate).ToList();

            if (lastComment != null)
                comments = comments.Where(c => c.DateCreate.CompareTo(lastComment.DateCreate) < 0).TakeLast(numComments).ToList();
            else
                comments = comments.TakeLast(numComments).ToList();

            return comments;
        }

        internal class ComparerComment : IComparer<Comment>
        {
           
            private readonly IUnitOfWork _unitOfWork;

            public ComparerComment(IUnitOfWork unitOfWork)
            {
                this._unitOfWork = unitOfWork;
            }
            public int Compare(Comment x, Comment y)
            {
                var days = x.DateCreate - y.DateCreate;

                var replies = x.Childs.Count() - y.Childs.Count();

                if ((days.Days * (3) + replies) > 0)
                    return 1;

                if ((days.Days * (3) + replies) < 0)
                    return -1;

                if (x.DateCreate > y.DateCreate)
                    return 1;

                if (x.DateCreate < y.DateCreate)
                    return -1;

                return 0;
            }
        }
    }
}

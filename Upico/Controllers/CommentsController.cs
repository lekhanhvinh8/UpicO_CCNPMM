using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Controllers.Resources;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Core.Services;

namespace Upico.Controllers
{
    [Route("api/comments")]
    //[Authorize]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentsController(IUnitOfWork unitOfWork, ICommentService commentService, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._commentService = commentService;
            this._mapper = mapper;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test(string postId)
        {
            var comments = await this._unitOfWork.Comments.GetAll();

            
            return Ok(comments);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string postId)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == postId);
            if (post == null)
                return BadRequest();

            await this._unitOfWork.Comments.Load(c => c.PostId == post.Id);

            foreach (var comment in post.Comments)
            {
                this._unitOfWork.Comments.LoadAllChildren(comment);
            }

            var result = this._mapper.Map<IList<Comment>, IList<CommentResouce>>(post.Comments);

            return Ok(SortComment(result));
        }

        [HttpGet("parents")]
        public async Task<IActionResult> GetParents(string postId, string lastCommentId, int numComments)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == postId);
            if (post == null)
                return BadRequest();

            var comments = await this._commentService.GetParentComments(postId, lastCommentId, numComments);

            var result = this._mapper.Map<IList<Comment>, IList<CommentDetailResource>>(comments);
            
            return Ok(result);
        }

        [HttpGet("childrens")]
        public async Task<IActionResult> GetChildrens(string parentId, string lastCommentId, int numComments)
        {
            var comment = await this._unitOfWork.Comments.SingleOrDefault(c => c.Id.ToString() == parentId);
            if (comment == null)
                return BadRequest();

            var comments = await this._commentService.GetChildrenComments(parentId, lastCommentId, numComments);

            var result = this._mapper.Map<IList<Comment>, IList<CommentDetailResource>>(comments);

            return Ok(result);
        }

        [HttpPost("comment")]
        public async Task<IActionResult> Comment([FromBody] CreatedCommentResouce commentResouce)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _unitOfWork.Users.GetUser(commentResouce.Username);
            if (user == null)
                return BadRequest();

            var post = await _unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == commentResouce.PostId);
            if (post == null)
                return BadRequest();

            var comment = new Comment();
            comment.Post = post;
            comment.User = user;
            comment.Content = commentResouce.Content;
            comment.DateCreate = DateTime.Now;

            //Load Avatar of user
            await this._unitOfWork.Avatars.Load(a => a.IsMain && a.UserID == user.Id);

            await this._unitOfWork.Comments.Add(comment);
            await this._unitOfWork.Complete();

            var result = this._mapper.Map<Comment, CommentDetailResource>(comment);

            return Ok(result);
        }

        [HttpPost("reply")]
        public async Task<IActionResult> Reply([FromBody] ReplyResource replyResouce)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _unitOfWork.Users.GetUser(replyResouce.Username);
            if (user == null)
                return BadRequest();

            var commentParent = await _unitOfWork.Comments.SingleOrDefault(c => c.Id.ToString() == replyResouce.ParentId);
            if (commentParent == null)
                return BadRequest();

            var comment = new Comment();
            comment.Parent = commentParent;
            comment.User = user;
            comment.Content = replyResouce.Content;
            comment.DateCreate = DateTime.Now;

            //Load Avatar of user
            await this._unitOfWork.Avatars.Load(a => a.IsMain && a.UserID == user.Id);

            await this._unitOfWork.Comments.Add(comment);
            await this._unitOfWork.Complete();

            var result = this._mapper.Map<Comment, CommentDetailResource>(comment);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var comment = await this._unitOfWork.Comments.SingleOrDefault(c => c.Id.ToString() == commentId);
            if (comment == null)
                return BadRequest();
            
            this._unitOfWork.Comments.RemoveAllChildren(commentId);
            this._unitOfWork.Comments.Remove(comment);
            await this._unitOfWork.Complete();

            return Ok();
        }

        private IList<CommentResouce> SortComment(IList<CommentResouce> comments)
        {
            return comments.OrderByDescending(c => c.Childs.Count).ToList();
        }

    }
}
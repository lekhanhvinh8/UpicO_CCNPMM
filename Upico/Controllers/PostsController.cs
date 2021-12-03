using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [Route("api/posts")]
    //[Authorize]
    [ApiController]

    public class PostsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public PostsController(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._userService = userService;
        }

        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetPosts(string userName)
        {
            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            //Load all posts of user
            await this._unitOfWork.Posts.Load(p => p.User.Id == user.Id);

            var posts = user.Posts;

            var result = this._mapper.Map<IList<Post>, IList<PostResouce>>(posts);

            return Ok(result);

        } 

        [HttpGet("relatedPosts")]
        public async Task<IActionResult> GetRelatedPosts(string username, int numPosts)
        {
            var user = await this._unitOfWork.Users.GetUserWithLikes(username);
            if (user == null)
                return NotFound();

            var posts = await this._unitOfWork.Posts.GetRelatedPosts(username, numPosts);

            var result = this._mapper.Map<IList<Post>, IList<DetailedPostResource>>(posts);

            AssignIsLikeProp(user, result);

            return Ok(result);
        }

        [HttpGet("moreRelatedPosts")]
        public async Task<IActionResult> GetRelatedPostsBefore(string username, string latestPostId, int numPosts)
        {
            var user = await this._unitOfWork.Users.GetUserWithLikes(username);
            if (user == null)
                return NotFound();

            var latestPost = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == latestPostId);
            if (latestPost == null)
                return NotFound();

            var posts = await this._unitOfWork.Posts.GetRelatedPostsBefore(username, latestPostId, numPosts);

            var result = this._mapper.Map<IList<Post>, IList<DetailedPostResource>>(posts);

            AssignIsLikeProp(user, result);

            return Ok(result);
        }

        [HttpGet("UserProfilePosts")]
        public async Task<IActionResult> getPosts(string sourceUsername, string targetUsername, int numPosts, bool getImages = true)
        {
            var sourceUser = await this._unitOfWork.Users.GetUserWithLikes(sourceUsername);
            var targetUser = await this._unitOfWork.Users.GetUser(targetUsername);

            if (sourceUser == null || targetUser == null)
                return BadRequest();

            var getPrivatePost = await this._userService.IsFollowed(sourceUsername, targetUsername);

            if (sourceUsername == targetUsername)
                getPrivatePost = true;

            IList<Post> posts = await this._unitOfWork.Posts.GetPosts(targetUsername, getPrivatePost, numPosts, getImages);

            var result = this._mapper.Map<IList<Post>, IList<PostUserProfileResource>>(posts);

            return Ok(result);
        }

        [HttpGet("MoreUserProfilePosts")]
        public async Task<IActionResult> getPostsBefore(string sourceUsername, string targetUsername, string latestPostId, int numPosts, bool getImages = true)
        {
            var sourceUser = await this._unitOfWork.Users.GetUserWithLikes(sourceUsername);
            var targetUser = await this._unitOfWork.Users.GetUser(targetUsername);

            if (sourceUser == null || targetUser == null)
                return BadRequest();

            var latestPost = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == latestPostId);
            if (latestPost == null)
                return NotFound();

            var getPrivatePost = await this._userService.IsFollowed(sourceUsername, targetUsername);

            if (sourceUsername == targetUsername)
                getPrivatePost = true;

            IList<Post> posts = await this._unitOfWork.Posts.GetPostsBefore(targetUsername, latestPostId, getPrivatePost, numPosts, getImages);

            var result = this._mapper.Map<IList<Post>, IList<PostUserProfileResource>>(posts);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPostDetail(string sourceUsername, string postId)
        {
            var user = await this._unitOfWork.Users.GetUserWithLikes(sourceUsername);
            if (user == null)
                return NotFound();

            var post = await this._unitOfWork.Posts.GetPostDetail(postId);
            if (post == null)
                return NotFound();

            var result = this._mapper.Map<Post, DetailedPostResource>(post);

            if (user.Likes.Select(l => l.Id).Contains(post.Id))
                result.IsLiked = true;

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreatePostResource postResource)
        {
            var user = await this._unitOfWork.Users.GetUser(postResource.UserName);
            if (user == null)
                return NotFound();

            var post = this._mapper.Map<CreatePostResource, Post>(postResource);
            post.DateCreate = DateTime.Now;

            user.Posts.Add(post);
            await this._unitOfWork.Complete();

            var detailedPost = await this._unitOfWork.Posts.GetPostDetail(post.Id.ToString());

            var result = this._mapper.Map<Post, DetailedPostResource>(detailedPost);
            return Ok(result);
        }

        [HttpPost("report")]
        public async Task<IActionResult> Report(ReportResource reportedPostResource)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == reportedPostResource.Id);
            var user = await this._unitOfWork.Users.GetUser(reportedPostResource.ReporterUserName);

            if (post == null || user == null)
                return NotFound();

            var reportedPostInDb = await this._unitOfWork.ReportedPosts.SingleOrDefault(r => r.PostId == post.Id && r.ReporterId == user.Id);
            if (reportedPostInDb != null)
                return BadRequest("you already have reported this post");

            var reportedPost = this._mapper.Map<ReportResource, Report>(reportedPostResource);
            reportedPost.Reporter = user;

            await this._unitOfWork.ReportedPosts.Add(reportedPost);

            await this._unitOfWork.Complete();

            var result = this._mapper.Map<Report, ReportResource>(reportedPost);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePrivateMode(string postId, bool privateMode)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id.ToString() == postId);
            if (post == null)
                return BadRequest();

            post.PrivateMode = privateMode;

            await this._unitOfWork.Complete();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var post = await this._unitOfWork.Posts.GetPostDetail(postId);
            if (post == null)
                return NotFound();

            if (post.PostImages.Count != 0)
                return BadRequest();

            foreach (var comment in post.Comments)
            {
                this._unitOfWork.Comments.RemoveAllChildren(comment.Id.ToString());
                this._unitOfWork.Comments.Remove(comment);
            }

            post.Reports.Clear();
            post.Likes.Clear();

            this._unitOfWork.Posts.Remove(post);

            await this._unitOfWork.Complete();

            return Ok();
        }

        private void AssignIsLikeProp(AppUser sourceUser, IList<DetailedPostResource> postResources)
        {
            foreach (var post in postResources)
            {
                if (sourceUser.Likes.Select(l => l.Id).Contains(post.Id))
                    post.IsLiked = true;
            }
        }
    }
}

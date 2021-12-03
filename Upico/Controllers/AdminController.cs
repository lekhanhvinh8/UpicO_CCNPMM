using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Controllers.Resources;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Core.Services;
using Upico.Core.StaticValues;

namespace Upico.Controllers
{
    //[Authorize(Roles =RoleNames.RoleAdmin)]
    [Route("/api/admin")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public AdminController(IUserService userService, IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            this._userService = userService;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._photoService = photoService;
        }

        [HttpGet("reports")]

        public async Task<IActionResult> GetReportedPost()
        {
            var reportedPosts = await this._unitOfWork.ReportedPosts.GetAll();

            var postIds = reportedPosts.Select(r => r.PostId).Distinct().ToList();

            var firstReports = new List<FirstReportedPostResource>();

            foreach (var postId in postIds)
            {
                var reportedPostsWithId = reportedPosts
                    .Where(r => r.PostId == postId)
                    .OrderBy(r => r.DateCreated);

                var numOfReport = reportedPostsWithId.Count();
                var firstReport = reportedPostsWithId.FirstOrDefault();

                var user = await this._unitOfWork.Users.SearchUserById(firstReport.ReporterId);

                var reportResource = new FirstReportedPostResource()
                {
                    
                    Id = firstReport.PostId,
                    NumOfReports = numOfReport,
                    FirstReportTime = firstReport.DateCreated,
                    FirtsReporter = firstReport.Reporter.UserName,
                };

                firstReports.Add(reportResource);
            }

            Response.Headers.Add("Content-Range", "post 0-20/" + firstReports.Count);
            return Ok(firstReports.OrderByDescending(f => f.FirstReportTime));
        }

        [HttpGet("reports/{id}")]
        public async Task<IActionResult> GetReportedPostDetail(string id)
        {
            var post = await this._unitOfWork.Posts.GetReportedPost(id);

            if (post == null)
                return BadRequest();

            var result = this._mapper.Map<Post, DetailReportedPostResource>(post);

            return Ok(result);
        }

        [HttpDelete("pass")]
        public async Task<IActionResult> PassAllReport(string postId)
        {
            var post = await this._unitOfWork.Posts.GetReportedPost(postId);
            if (post == null)
                return BadRequest();

            post.Reports.Clear();
            await this._unitOfWork.Complete();

            return Ok();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var post = await this._unitOfWork.Posts.GetReportedPost(postId);
            if (post == null)
                return BadRequest();

            //Load all images and delete it first of post
            await this._unitOfWork.PostedImages.Load(p => p.PostId == post.Id);

            var postedImages = post.PostImages;
            var postedImageIds = postedImages.Select(p => p.Id).ToList();

            await this._photoService.DeletePhotos(postedImageIds);

            post.PostImages.Clear();
            this._unitOfWork.PostedImages.RemoveRange(postedImages);

            //Delete all related infomation
            post.Reports.Clear();
            post.Likes.Clear();

            this._unitOfWork.Comments.RemoveAllComments(postId);

            //Finally delete the post
            this._unitOfWork.Posts.Remove(post);

            await this._unitOfWork.Complete();

            return Ok();
        }
    }
}

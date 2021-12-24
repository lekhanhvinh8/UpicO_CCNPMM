using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Upico.Controllers.Resources;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Core.ServiceResources;
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
        private readonly IAdminService _adminService;

        public AdminController(IUserService userService, IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService, IAdminService adminService)
        {
            this._userService = userService;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._photoService = photoService;
            this._adminService = adminService;
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

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string filter)
        {
            var lightWeightUsers = new List<Object>();

            var userFilter = string.IsNullOrEmpty(filter) ? null : JsonConvert.DeserializeObject<UserFilter>(WebUtility.UrlDecode(filter));
            var users = await this._unitOfWork.Users.GetAll(userFilter);

            foreach (var user in users)
            {
                var avatars = await this._unitOfWork.Avatars.GetAvatars(user.UserName);
                var avatarUrl = "";

                if (avatars.Where(a => a.IsMain).Count() != 0)
                {
                    avatarUrl = avatars.Where(a => a.IsMain).ToList()[0].Path;
                }


                if (user.UserName != "admin")
                lightWeightUsers.Add(new { 
                    id= user.Id,
                    userName= user.UserName,
                    bio = user.Bio,
                    displayName = user.DisplayName,
                    email = user.Email,
                    isLocked = user.isLock,
                    avatarUrl = avatarUrl,
                });
            }

            Response.Headers.Add("Content-Range", "user 0-1/" + lightWeightUsers.Count);
            return Ok(lightWeightUsers);
        }

        [HttpPut("UpdateUserStatus")]

        public async Task<IActionResult> ModifyUserStatus(string userName, bool isLock)
        {
            var user = await this._unitOfWork.Users.GetUser(userName);
            user.isLock = isLock;
            await this._unitOfWork.Complete();

            return Ok();
        }

        [HttpGet("DashboardInfo")]
        public async Task<IActionResult> GetDashboardInfo()
        {
            var totalPosts = await this._unitOfWork.Posts.Count();
            var totalComments = await this._unitOfWork.Comments.Count();
            var totalUsers = await this._unitOfWork.Users.Count();
            var totalMessages = await this._adminService.CountMessage();
            var totalLikes = await this._adminService.CountLike();
            var totalAccess = await this._adminService.CountAccess();
            var newUsers = await this._adminService.GetNewUser(10);
            var onlineUsers = await this._adminService.GetOnlineUser(10);


            return Ok(
                new { 
                    totalPosts, 
                    totalComments, 
                    totalUsers, 
                    totalMessages, 
                    totalLikes, 
                    totalAccess, 
                    newUsers = this._mapper.Map<IList<UserResource>>(newUsers), 
                    onlineUsers = this._mapper.Map<IList<UserResource>>(onlineUsers),
                });
        }

        [HttpGet("GetDateAccessCount")]
        public async Task<IActionResult> GetDateAccessCount (DateTime date)
        {
            return Ok(await this._adminService.GetDateAccessCount(date));
        }

        [HttpGet("GetMonthAccessCount")]
        public async Task<IActionResult> GetMonthAccessCount (DateTime date)
        {
            return Ok(await this._adminService.GetMonthAccessCount(date));
        }

        [HttpGet("GetYearAccessCount")]
        public async Task<IActionResult> GetYearAccessCount (DateTime date)
        {
            return Ok(await this._adminService.GetYearAccessCount(date));
        }


        [HttpGet("GetDateNewUsersCount")]
        public async Task<IActionResult> GetDateNewUsersCount(DateTime date)
        {
            return Ok(await this._adminService.GetDateNewUsers(date));
        }

        [HttpGet("GetMonthNewUsersCount")]
        public async Task<IActionResult> GetMonthNewUsersCount(DateTime date)
        {
            return Ok(await this._adminService.GetMonthNewUsers(date));
        }

        [HttpGet("GetYearNewUsersCount")]
        public async Task<IActionResult> GetYearNewUsersCount(DateTime date)
        {
            return Ok(await this._adminService.GetYearNewUsers(date));
        }

        [HttpGet("GenerateAccessData")]
        public async Task<IActionResult> GenerateAccessData(string username, DateTime date, string mode, int numberOfData)
        {
            await this._adminService.GenerateAccessData(username, date, mode, numberOfData);
            return Ok();
        }

        [HttpGet("GenerateAccountData")]
        public async Task<IActionResult> GenerateAccountData(DateTime date, string mode, int numberOfData)
        {
            await this._adminService.GenerateAccountData( date,  mode,  numberOfData);
            return Ok();
        }

    }
}

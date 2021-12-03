using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Upico.Controllers.Resources;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Core.Services;

namespace Upico.Controllers
{
    [Route("/api/avatars/{userName}")]
    [Authorize]
    [ApiController]
    public class AvatarsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public AvatarsController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string userName)
        {
            if (!IsUser(userName))
                return Unauthorized();

            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            var avatars = await this._unitOfWork.Avatars.GetAvatars(userName);

            await this._unitOfWork.Complete();

            var result = this._mapper.Map<List<Avatar>, List<AvatarResource>>(avatars);

            return Ok(result);
        }

        [HttpGet("main")]
        public async Task<IActionResult> GetMain(string userName)
        {
            if (!IsUser(userName))
                return Unauthorized();

            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            var avatar = await this._unitOfWork.Avatars.SingleOrDefault(a => a.UserID == user.Id && a.IsMain == true);
            if (avatar == null)
                return NotFound();

            var result = this._mapper.Map<Avatar, AvatarResource>(avatar);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(string userName, IFormFile file)
        {
            if (!IsUser(userName))
                return Unauthorized();

            if (file == null)
                return BadRequest("no file");

            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            //Get main avatar of user
            var oldAvatar = await this._unitOfWork.Avatars.SingleOrDefault(a => a.UserID == user.Id && a.IsMain == true);
            if (oldAvatar != null)
                oldAvatar.IsMain = false;

            //Upload new photo to clound
            var cloudPhoto = await this._photoService.AddPhoto(file);

            //Create a new photo in db
            var newAvatar = new Avatar()
            {
                Id = cloudPhoto.Id,
                Path = cloudPhoto.Url,
                UploadTime = DateTime.Now,
                IsMain = true,
            };
            user.Avatars.Add(newAvatar);

            await this._unitOfWork.Complete();

            return Ok(this._mapper.Map<Avatar, AvatarResource>(newAvatar));
        }

        [HttpDelete("{photoID}")]
        public async Task<IActionResult> DeleteAvatar(string userName, string photoId)
        {
            if (!IsUser(userName))
                return Unauthorized();

            var user = await this._unitOfWork.Users.GetUser(userName);
            if (user == null)
                return NotFound();

            //Load all avatars of user
            await this._unitOfWork.Avatars.Load(a => a.UserID == user.Id);

            var avatar = user.Avatars.FirstOrDefault(a => a.Id == photoId);
            if (avatar == null)
                return NotFound();

            if (avatar.IsMain)
                return BadRequest("Can not delete main avatar");

            // Delete photo from cloud service
            var result = await this._photoService.DeletePhoto(photoId);

            if (result == null)
                return Problem();

            // Delete photo from database
            user.Avatars.Remove(avatar);
            this._unitOfWork.Avatars.Remove(avatar);

            await this._unitOfWork.Complete();

            return Ok();
        }

        private bool IsUser(string userName)
        {
            var user = User;
            var claimName = user.FindFirst(ClaimTypes.Name);

            if (claimName.Value != userName)
                return false;

            return true;
        }
        
    }
}

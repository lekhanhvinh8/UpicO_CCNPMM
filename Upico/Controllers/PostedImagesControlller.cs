using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Controllers.Resouces;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Core.Services;

namespace Upico.Controllers
{
    [Route("api/postedImages/{postId}")]
    //[Authorize]
    [ApiController]
    public class PostedImagesControlller : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public PostedImagesControlller(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._photoService = photoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPhotos(string postId)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id == new Guid(postId));
            if (post == null)
                return NotFound();

            //Load all photos of post
            await this._unitOfWork.PostedImages.Load(i => i.PostId == post.Id);

            var photos = this._mapper.Map<IList<PostedImage>, IList<PhotoResource>>(post.PostImages);

            return Ok(photos);
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotos(string postId, IFormCollection files)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id == new Guid(postId));
            if (post == null)
                return NotFound();

            if (files.Files.Count == 0)
                return BadRequest("No file");

            var photos = await this._photoService.AddPhotos(files);

            foreach (var photo in photos)
            {
                var postedImage = new PostedImage()
                {
                    Id = photo.Id,
                    Path = photo.Url,
                };

                post.PostImages.Add(postedImage);
            }

            await this._unitOfWork.Complete();

            var result = this._mapper.Map<IList<PostedImage>, IList<PhotoResource>>(post.PostImages);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePhotos(string postId)
        {
            var post = await this._unitOfWork.Posts.SingleOrDefault(p => p.Id == new Guid(postId));
            if (post == null)
                return NotFound();

            //Load all images of post
            await this._unitOfWork.PostedImages.Load(p => p.PostId == post.Id);

            var postedImages = post.PostImages;
            var postedImageIds = postedImages.Select(p => p.Id).ToList();

            await this._photoService.DeletePhotos(postedImageIds);

            post.PostImages.Clear();
            this._unitOfWork.PostedImages.RemoveRange(postedImages);

            await this._unitOfWork.Complete();

            return Ok();
        }
    }
}

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Upico.Controllers.Resouces;
using Upico.Core.Services;

namespace Upico.Persistence.Service
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
                );

            _cloudinary = new Cloudinary(account);
        }
        public async Task<PhotoResource> AddPhoto(IFormFile file)
        {
            if(file.Length > 0)
            {
                await using var stream = file.OpenReadStream();

                try
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                        throw new Exception(uploadResult.Error.Message);

                    return new PhotoResource
                    {
                        Id = uploadResult.PublicId,
                        Url = uploadResult.SecureUrl.ToString()
                    };
                }
                catch(System.Net.Http.HttpRequestException e)
                {
                    Console.WriteLine(e);
                }
                
            }

            return null;
        }

        public async Task<IList<PhotoResource>> AddPhotos(IFormCollection files)
        {
            var photos = new List<PhotoResource>();

            foreach (var file in files.Files)
            {
                var photo = await AddPhoto(file);
                photos.Add(photo);
            }
            
            return photos;
        }

        public async Task<string> DeletePhoto(string id)
        {
            var deleteParams = new DeletionParams(id);

            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            return deleteResult.Result == "ok" ? deleteResult.Result : null;
        }

        public async Task<string> DeletePhotos(IList<string> ids)
        {
            foreach (var id in ids)
            {
                var deleteResult = await DeletePhoto(id);

                if (deleteResult == null)
                    throw new Exception("An error occur when deleting a photo from coundinary");

            }

            return "ok";
        }
    }
}

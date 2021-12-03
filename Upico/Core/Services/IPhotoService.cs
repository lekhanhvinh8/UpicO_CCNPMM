using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Upico.Controllers.Resouces;

namespace Upico.Core.Services
{
    public interface IPhotoService
    {
        public Task<PhotoResource> AddPhoto(IFormFile file);
        public Task<IList<PhotoResource>> AddPhotos(IFormCollection files);
        public Task<string> DeletePhoto(string id);
        public Task<string> DeletePhotos(IList<string> ids);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Upico.Controllers.Resources;
using Upico.Core.Domain;

namespace Upico.Core.Repositories
{
    public interface IUserRepository
    {
        public Task<AppUser> GetUser(string userName);
        public Task<AppUser> GetUserWithLikes(string userName);
        public Task<AppUser> SearchUserByUsername(string username);
        public Task<AppUser> SearchUserById(string id);
        public Task<List<AppUser>> SearchUsersByDisplayName(string displayName);
        public Task LoadMainAvatar(string userName);
        public Task LoadFollowings(string username);
        public Task LoadFollowers(string username);
        public Task<AppUser> GetUserProfile(string username);
        Task Load(Expression<Func<AppUser, bool>> predicate);
    }
}

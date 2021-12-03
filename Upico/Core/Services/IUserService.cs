using System.Collections.Generic;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.ServiceResources;

namespace Upico.Core.Services
{
    public interface IUserService
    {
        public Task<LoginResponse> Authenticate(LoginRequest request);
        public Task<IList<string>> Register(RegisterRequest request);
        public Task<AppUser> GetUser(string userName);
        public Task<List<AppUser>> SearchUser(string key);
        public Task<bool> IsFollowed(string followerUsername, string followingUsername);
        public Task SendChangeEmailRequest(string username, string newEmail, string callbackurl);
        public Task<bool> ConfirmChangeEmail(string username, string newEmail, string token);
        public Task<bool> ChangePassword(string userName, string currentPassword, string newPassword);
        public Task<bool> CheckPassword(string userName, string password);
        public Task<IList<AppUser>> GetFollowingSuggestion(string userName, int numFollowings);

        public Task SendConfirmEmailRequest(string username, string callbackUrl);
        public Task<bool> ConfirmEmail(string username, string token);
    }
}

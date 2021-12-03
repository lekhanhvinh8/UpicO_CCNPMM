using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Upico.Core;
using Upico.Core.Domain;
using Upico.Core.ServiceResources;
using Upico.Core.Services;
using Upico.Core.StaticValues;

namespace Upico.Persistence.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;

        public UserService(UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IUnitOfWork unitOfWork,
            IMailService mailService,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
            _config = config;
        }

        public async Task<LoginResponse> Authenticate(LoginRequest request)
        {
            // check user exitst
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return null;

            //check password
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
                return null;

            //get roles
            var roles = await _userManager.GetRolesAsync(user);

            var fullName = user.FullName;
            if (fullName == null)
                fullName = "undefined";
            //create claims
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName, fullName),
                new Claim(ClaimTypes.Name,user.UserName)
            };
            foreach (var i in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, i));
            }
            
            //create token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity((claims)),
                Expires = DateTime.Now.AddMonths(2),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var finalToken = tokenHandler.WriteToken(token);

            //Load Avatar
            await this._unitOfWork.Avatars.Load(a => a.UserID == user.Id && a.IsMain);

            var avatar = user.Avatars.Where(a => a.IsMain).FirstOrDefault();
            string path = null;

            if (avatar != null)
                path = avatar.Path;

            var loginResponse = new LoginResponse()
            {
                UserName = user.UserName,
                Token = finalToken,
                DisplayName = user.DisplayName,
                AvatarUrl = path,
                RoleName = roles.FirstOrDefault()
            };

            return loginResponse;
            /*
            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds);

            //return token
            return new JwtSecurityTokenHandler().WriteToken(token);
            */
        }

        public async Task<AppUser> GetUser(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IList<string>> Register(RegisterRequest request)
        {

            var listError = new List<string>();

            var user = await _userManager.FindByNameAsync(request.Username);

            //check email
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                listError.Add("Email already in use");
            }

            //check username
            if (user != null)
            {
                listError.Add("Username already exists");
            }

            user = new AppUser()
            {
                UserName = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                FullName = request.FullName,
                Email = request.Email
            };
            if (listError.Count != 0)
                return listError;

            var result = await _userManager.CreateAsync(user, request.Password);
            await _userManager.AddToRoleAsync(user, RoleNames.RoleUser);

            if (result.Succeeded)
                return null;

            throw new Exception("Error when creating user!! oh yeah");
        }

        public async Task<List<AppUser>> SearchUser(string key)
        {
            var users = new List<AppUser>();

            var user = await this._unitOfWork.Users.SearchUserById(key);
            if (user != null)
            {
                users.Add(user);
            }

            user = await this._unitOfWork.Users.SearchUserByUsername(key);
            if (user != null)
            {
                users.Add(user);
            }

            if (users.Count == 1)
            {
                if(await IsOnlyRoleUser(users[0]))
                {
                    await this._unitOfWork.Users.LoadMainAvatar(users[0].UserName);

                    return users;
                }    

            }

            users = await this._unitOfWork.Users.SearchUsersByDisplayName(key);

            var usersRoleUser = new List<AppUser>();
            foreach (var user1 in users)
            {
                if(await IsOnlyRoleUser(user1))
                {
                    usersRoleUser.Add(user1);
                }
            }

            foreach (var userRoleUser in usersRoleUser)
            {
                await this._unitOfWork.Users.LoadMainAvatar(userRoleUser.UserName);
            }


            return usersRoleUser;
        }

        private async Task<bool> IsOnlyRoleUser(AppUser user)
        {
            //Only role User or not has any roles

            var roles = await this._userManager.GetRolesAsync(user);

            bool flag = true;
            foreach (var role in roles)
            {
                if (role != RoleNames.RoleUser)
                    flag = false;
            }

            return flag;
        }

        public async Task<bool> IsFollowed(string followerUsername, string followingUsername)
        {
            var follower = await this._unitOfWork.Users.GetUser(followerUsername);
            var following = await this._unitOfWork.Users.GetUser(followingUsername);

            await this._unitOfWork.Users.LoadFollowings(follower.UserName);

            return follower.Followings.Contains(following);
        }

        public async Task SendConfirmEmailRequest(string username, string callbackUrl)
        {
            var user = await this._userManager.FindByNameAsync(username);
            var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

            callbackUrl += String.Format("?username={0}&token={1}", user.UserName, HttpUtility.UrlEncode(token));

            var htmlContent = String.Format(
                    @"To Register. Please confirm the email by clicking this link: 
        <br><a href='{0}'>Confirm new email</a>",
                    callbackUrl);

            // send email to the user with the confirmation link
            MailRequest mailRequest = new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Hello From Upico",
                Body = htmlContent
            };

            try
            {
                await this._mailService.SendEmailAsync(mailRequest);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<bool> ConfirmEmail(string username, string token)
        {
            var user = await this._userManager.FindByNameAsync(username);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task SendChangeEmailRequest(string username, string newEmail, string callbackurl)
        {
            var user = await this._userManager.FindByNameAsync(username);

            var token = await this._userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            // append userId and confirmation code as parameters to the url
            callbackurl += String.Format("?username={0}&newEmail={1}&token={2}", user.UserName, newEmail, HttpUtility.UrlEncode(token));

            var htmlContent = String.Format(
                    @"To change your email. Please confirm the email by clicking this link: 
        <br><a href='{0}'>Confirm new email</a>",
                    callbackurl);

            // send email to the user with the confirmation link
            MailRequest mailRequest = new MailRequest()
            {
                ToEmail = user.Email,
                Subject = "Change email",
                Body = htmlContent
            };

            try
            {
                await this._mailService.SendEmailAsync(mailRequest);

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<bool> ConfirmChangeEmail(string username, string newEmail, string token)
        {
            var user = await this._userManager.FindByNameAsync(username);

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);


            return result.Succeeded;
        }

        public async Task<bool> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            var user = await this._userManager.FindByNameAsync(userName);

            var result = await this._userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return result.Succeeded;
        }

        public async Task<bool> CheckPassword(string userName, string password)
        {
            var user = await this._userManager.FindByNameAsync(userName);

            var result = await this._userManager.CheckPasswordAsync(user, password);

            return result;
        }

        public async Task<IList<AppUser>> GetFollowingSuggestion(string userName, int numFollowings)
        {
            var user = await this._unitOfWork.Users.GetUser(userName);

            await this._unitOfWork.Users.LoadFollowings(user.UserName);

            foreach (var following in user.Followings)
            {
                await this._unitOfWork.Users.LoadFollowings(following.UserName);
            }

            var successors = user.Followings.SelectMany(f => f.Followings).Distinct().ToList();

            successors = successors.Where(s => s.UserName != user.UserName && !user.Followings.Select(f => f.UserName).Contains(s.UserName)).ToList();

            successors = successors.OrderByDescending(s => s, new CompareFollowing(user, _unitOfWork)).Take(numFollowings * 2).ToList();

            return successors.OrderBy(s => Guid.NewGuid()).Take(numFollowings).ToList();
        }

        internal class CompareFollowing : IComparer<AppUser>
        {
            private readonly AppUser _sourceUser;
            private readonly IUnitOfWork _unitOfWork;

            public CompareFollowing(AppUser sourceUser, IUnitOfWork unitOfWork)
            {
                this._sourceUser = sourceUser;
                this._unitOfWork = unitOfWork;
            }
            //SortBySalaryByAscendingOrder
            public int Compare(AppUser x, AppUser y)
            {
                //Load Followers of user x
                var asyncMethod = this._unitOfWork.Users.LoadFollowers(x.UserName);
                asyncMethod.Wait();

                //Load Followers of user y
                asyncMethod = this._unitOfWork.Users.LoadFollowers(y.UserName);
                asyncMethod.Wait();

                //Load Followings of source user
                asyncMethod = this._unitOfWork.Users.LoadFollowings(_sourceUser.UserName);
                asyncMethod.Wait();

                int scoreUserX = 0;
                int scoreUserY = 0;

                foreach (var user in _sourceUser.Followings)
                {
                    var followings = user.Followings.Select(u => u.UserName);

                    if (followings.Contains(x.UserName))
                        scoreUserX++;
                    if (followings.Contains(y.UserName))
                        scoreUserY++;
                }

                if (scoreUserX > scoreUserY)
                    return 1;

                if (scoreUserX < scoreUserY)
                    return -1;

                return 0;
            }

            
        }

    }
}

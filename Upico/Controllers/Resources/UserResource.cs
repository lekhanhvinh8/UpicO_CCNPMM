using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upico.Controllers.Resources
{
    public class UserResource
    {
        public string Id { set; get; }
        public string UserName { get; set; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string FullName { set; get; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public int Followers { get; set; }
        public int Followings { get; set; }
        public int Posts { get; set; }
        public string AvatarUrl { get; set; }
        public bool isFollowed { get; set; }
    }
}

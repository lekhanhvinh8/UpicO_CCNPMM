using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Upico.Core.Domain
{
    public class AppUser : IdentityUser
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string FullName { set; get; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public IList<Post> Posts { set; get; }
        public IList<Avatar> Avatars { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<Post> Likes { get; set; }
        public IList<AppUser> Followers { get; set; }
        public IList<AppUser> Followings { get; set; }
        public IList<Report> Reports { get; set; }
        public AppUser()
        {
            Posts = new List<Post>();
            this.Avatars = new List<Avatar>();
            Comments = new List<Comment>();
            Likes = new List<Post>();
            Followers = new List<AppUser>();
            Followings = new List<AppUser>();
            Reports = new List<Report>();
        }
    }
}

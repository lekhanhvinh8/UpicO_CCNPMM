using AutoMapper;
using System;
using System.Linq;
using Upico.Controllers.Resouces;
using Upico.Controllers.Resources;
using Upico.Core.Domain;

namespace Upico.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Avatar, AvatarResource>();
            CreateMap<AppUser, UserResource>()
                .ForMember(ur => ur.AvatarUrl, opt => opt.MapFrom(u => u.Avatars.FirstOrDefault(a => a.IsMain) != null ? u.Avatars.FirstOrDefault(a => a.IsMain).Path : null))
                .ForMember(ur => ur.Followers, opt => opt.MapFrom(u => u.Followers.Count))
                .ForMember(ur => ur.Followings, opt => opt.MapFrom(u => u.Followings.Count))
                .ForMember(ur => ur.Posts, opt => opt.MapFrom(u => u.Posts.Count));
            CreateMap<AppUser, LightweightUserResource>()
                .ForMember(lr => lr.AvatarUrl, opt => opt.MapFrom(u => u.Avatars.FirstOrDefault(a => a.IsMain) != null ? u.Avatars.FirstOrDefault(a => a.IsMain).Path : null));
            CreateMap<PostedImage, PhotoResource>()
                .ForMember(pt => pt.Url, opt => opt.MapFrom(pi => pi.Path));
            CreateMap<Post, PostResouce>();
            CreateMap<Post, PostUserProfileResource>()
                .ForMember(dp => dp.Likes, opt => opt.MapFrom(p => p.Likes.Count))
                .ForMember(dp => dp.Comments, opt => opt.MapFrom(p => p.Comments.Count));
            CreateMap<Post, DetailedPostResource>()
                .ForMember(dp => dp.Comments, opt => opt.MapFrom(p => p.Comments.Count))
                .ForMember(dp => dp.Likes, opt => opt.MapFrom(p => p.Likes.Count()))
                .ForMember(dp => dp.DisplayName, opt => opt.MapFrom(p => p.User.DisplayName))
                .ForMember(dp => dp.Username, opt => opt.MapFrom(p => p.User.UserName))
                .ForMember(dp => dp.AvatarUrl, opt => opt.MapFrom(p => p.User.Avatars.FirstOrDefault(a => a.IsMain) != null ? p.User.Avatars.FirstOrDefault(a => a.IsMain).Path : null));
            CreateMap<AppUser, SearchUserResource>()
                .ForMember(dp => dp.AvatarUrl, opt => opt.MapFrom(p => p.Avatars.FirstOrDefault(a => a.IsMain) != null ? p.Avatars.FirstOrDefault(a => a.IsMain).Path : null));
            CreateMap<Comment, CommentDetailResource>()
                .ForMember(cr => cr.Username, opt => opt.MapFrom(c => c.User.UserName))
                .ForMember(cr => cr.UserDisplayName, opt => opt.MapFrom(c => c.User.DisplayName))
                .ForMember(cr => cr.Replies, opt => opt.MapFrom(c => c.Childs.Count()))
                .ForMember(cr => cr.UserAvatarUrl, opt => opt.MapFrom(c => c.User.Avatars.FirstOrDefault(a => a.IsMain) != null ? c.User.Avatars.FirstOrDefault(a => a.IsMain).Path : null));
            CreateMap<Comment, CommentResouce>()
                .ForMember(cr => cr.Childs, opt => opt.Ignore())
                .AfterMap((c, cr) => {
                    MapChildren(c, cr);
                });
            CreateMap<Report, ReportResource>()
                .ForMember(rr => rr.ReporterUserName, opt => opt.MapFrom(r => r.Reporter.UserName))
                .ForMember(rr => rr.Id, opt => opt.MapFrom(r => r.PostId));
            CreateMap<Post, DetailReportedPostResource>()
                .ForMember(dr => dr.UserName, opt => opt.MapFrom(p => p.User.UserName));

            CreateMap<CreatePostResource, Post>();
            CreateMap<UpdateUserProfieResource, AppUser>()
                .ForMember(u => u.UserName, opt => opt.Ignore())
                .ForMember(u => u.FullName, opt => opt.MapFrom(ur => ur.FirstName + " " + ur.LastName));
            CreateMap<ReportResource, Report>()
                .ForMember(r => r.PostId, opt => opt.MapFrom(rr => new Guid(rr.Id)))
                .ForMember(r => r.DateCreated, opt => opt.MapFrom(rr => DateTime.Now));
        }
        private void MapChildren(Comment comment, CommentResouce commentResouce)
        {
            //basic mapping
            commentResouce.Id = comment.Id;
            commentResouce.Content = comment.Content;
            commentResouce.DateCreate = comment.DateCreate;
            commentResouce.Username = comment.User.UserName;
            commentResouce.UserDisplayName = comment.User.DisplayName;

            var avatar = comment.User.Avatars.FirstOrDefault(a => a.IsMain);

            if(avatar != null)
                commentResouce.UserAvatarUrl = avatar.Path;
            else
                commentResouce.UserAvatarUrl = null;

            //Child mapping
            if (comment.Childs.Count == 0)
                return;

            foreach (var child in comment.Childs)
            {
                var commentResourceChild = new CommentResouce();
                MapChildren(child, commentResourceChild);

                commentResouce.Childs.Add(commentResourceChild);
            }

        }
    }
}

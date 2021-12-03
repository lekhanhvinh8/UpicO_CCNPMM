using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;
using Upico.Core.Repositories;

namespace Upico.Persistence.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly UpicODbContext _context;

        public PostRepository(UpicODbContext context)
            :base(context)
        {
            _context = context;
        }
        public async Task<Post> GetPostDetail(string postId)
        {
            var post = await this._context.Posts.Include(p => p.Likes)
                                                .Include(p => p.Comments)
                                                .Include(p => p.PostImages)
                                                .Include(p => p.User)
                                                .Include(p => p.User.Avatars.Where(a => a.IsMain))
                                                .SingleOrDefaultAsync(p => p.Id.ToString() == postId);

            return post;
        }
        public async Task<Post> GetReportedPost(string postId)
        {
            var post = await this._context.Posts
                .Include(p => p.Reports)
                    .ThenInclude(r => r.Reporter)
                .Include(p => p.PostImages)
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.Id.ToString() == postId && p.Reports.Count > 0);

            return post;
        }
        private async Task<IQueryable<Post>> GetRelatedPosts(string userName)
        {
            var user = await this._context.Users
                .Include(u => u.Avatars.Where(a => a.IsMain))
                .Include(u => u.Followings).ThenInclude(f => f.Avatars.Where(a => a.IsMain))
                .SingleOrDefaultAsync(u => u.UserName == userName);

            var relatedPost =  this._context.Posts.Where(p => p.UserId == user.Id || user.Followings.Select(u => u.Id).Contains(p.UserId));

            return relatedPost;
        }

        public async Task<IList<Post>> GetRelatedPosts(string userName, int numPosts)
        {
            var posts = await GetRelatedPosts(userName);

            posts = posts.Include(p => p.Likes);

            /*
            await this._context.Users.Where(u => u.Likes
            .Select(l => l.Id)
            .Intersect(posts.Select(p => p.Id))
            .Any()).LoadAsync();
            */

            await this._context.Comments.Where(c => posts.Select(p => p.Id).Contains(c.Post.Id)).LoadAsync();

            await this._context.PostImages.Where(pi => posts.Select(p => p.Id).Contains(pi.PostId)).LoadAsync();

            var newPosts = await posts
                .OrderByDescending(p => p.DateCreate)
                .Take(numPosts)
                .ToListAsync();

            /*
            var newPosts = await posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostImages)
                .OrderByDescending(p => p.DateCreate)
                .Take(numPosts)
                .ToListAsync();
            */

            return newPosts;
            
        }

        public async Task<IList<Post>> GetRelatedPostsBefore(string userName, string latestPostId, int numPosts)
        {
            var latestPost = await this._context.Posts.SingleOrDefaultAsync(p => p.Id.ToString() == latestPostId);

            var posts = await GetRelatedPosts(userName);
            
            var newPosts = await posts.Where(p => p.DateCreate < latestPost.DateCreate)
                .OrderByDescending(p => p.DateCreate)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostImages)
                .Take(numPosts)
                .ToListAsync();

            return newPosts;
        }

        public async Task<IList<Post>> GetPosts(string username, bool getPrivatePost, int numPosts, bool getImages)
        {
            var user = await this._context.Users
                .SingleOrDefaultAsync(u => u.UserName == username);

            var posts = await this._context.Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostImages)
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.DateCreate)
                .Take(numPosts)
                .ToListAsync();

            if (posts.Count == 0)
                return posts;

            IList<Post> filterPosts = new List<Post>(posts);

            if (!getPrivatePost)
                filterPosts = posts.Where(p => p.PrivateMode == false).ToList();

            if(getImages)
                filterPosts = filterPosts.Where(p => p.PostImages.Count > 0).ToList();
            else
                filterPosts = filterPosts.Where(p => p.PostImages.Count == 0).ToList();


            if (filterPosts.Count < numPosts)
            {
                var additionalPosts = await GetPostsBefore(username,
                    posts.LastOrDefault().Id.ToString(),
                    getPrivatePost,
                    numPosts - filterPosts.Count,
                    getImages);

                foreach (var post in additionalPosts)
                {
                    filterPosts.Add(post);
                }
            }    

            return filterPosts;
        }

        public async Task<IList<Post>> GetPostsBefore(string username, string latestPostId, bool getPrivatePost, int numPosts, bool getImages)
        {
            var user = await this._context.Users
                .SingleOrDefaultAsync(u => u.UserName == username);

            var latestPost = await this._context.Posts.SingleOrDefaultAsync(p => p.Id.ToString() == latestPostId);

            var posts = await this._context.Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Include(p => p.PostImages)
                .Where(p => p.UserId == user.Id && p.DateCreate < latestPost.DateCreate)
                .OrderByDescending(p => p.DateCreate)
                .Take(numPosts)
                .ToListAsync();

            if (posts.Count == 0)
                return posts;

            IList<Post> filterPosts = new List<Post>(posts);

            if (!getPrivatePost)
                filterPosts = posts.Where(p => p.PrivateMode == false).ToList();

            if(getImages)
                filterPosts = filterPosts.Where(p => p.PostImages.Count > 0).ToList();
            else
                filterPosts = filterPosts.Where(p => p.PostImages.Count == 0).ToList();

            if (filterPosts.Count < numPosts)
            {
                var additionalPosts = await this.GetPostsBefore(username, 
                    posts.LastOrDefault().Id.ToString(), 
                    getPrivatePost, 
                    numPosts - filterPosts.Count,
                    getImages);

                foreach (var post in additionalPosts)
                {
                    filterPosts.Add(post);
                }
            }

            return filterPosts;
        }

        
    }
}

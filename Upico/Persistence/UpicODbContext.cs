using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Upico.Core.Domain;
using Upico.Persistence.EntityConfigurations;

namespace Upico.Persistence
{
    public class UpicODbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostedImage> PostImages { get; set; }
        public DbSet<Report> ReportedPosts { get; set; }
        public UpicODbContext(DbContextOptions<UpicODbContext> options)
            :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new AvatarConfigurations());
            builder.ApplyConfiguration(new CommentConfigurations());
            builder.ApplyConfiguration(new PostConfigurations());
            builder.ApplyConfiguration(new PostImageConfigurations());
            builder.ApplyConfiguration(new AppUserConfiguration());
            builder.ApplyConfiguration(new ReportedPostConfiguration());
        }
    }
}

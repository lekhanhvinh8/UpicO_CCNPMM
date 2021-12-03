using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;

namespace Upico.Persistence.EntityConfigurations
{
    public class PostImageConfigurations : IEntityTypeConfiguration<PostedImage>
    {
        public void Configure(EntityTypeBuilder<PostedImage> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Post)
                .WithMany(au => au.PostImages)
                .HasForeignKey(a => a.PostId);
                

            builder.Property(a => a.Path).IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;

namespace Upico.Persistence.EntityConfigurations
{
    public class AvatarConfigurations : IEntityTypeConfiguration<Avatar>
    {
        public void Configure(EntityTypeBuilder<Avatar> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.AppUser)
                .WithMany(au => au.Avatars)
                .HasForeignKey(a => a.UserID);
                

            builder.Property(a => a.Path).IsRequired();
        }
    }
}

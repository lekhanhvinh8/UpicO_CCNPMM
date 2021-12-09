using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upico.Core.Domain;

namespace Upico.Persistence.EntityConfigurations
{
    public class MessageHubConfigurations : IEntityTypeConfiguration<MessageHub>
    {
        public void Configure(EntityTypeBuilder<MessageHub> builder)
        {
            builder.HasKey(mh => mh.Id);

            builder.HasIndex(mh => new { mh.SenderId, mh.ReceiverId }).IsUnique();

            builder.HasOne(mh => mh.Sender)
                .WithMany(u => u.SenderMessageHubs)
                .HasForeignKey(mh => mh.SenderId);

            builder.HasOne(mh => mh.Receiver)
                .WithMany(u => u.ReceiverMessageHubs)
                .HasForeignKey(mh => mh.ReceiverId);
        }
    }
}

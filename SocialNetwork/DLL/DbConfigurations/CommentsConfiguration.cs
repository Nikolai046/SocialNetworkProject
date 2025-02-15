using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.DbConfigurations;

public class CommentsConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
     
        builder.ToTable("Comments");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).UseIdentityColumn();

        builder
            .HasOne(c => c.Message)
            .WithMany(m => m.Comments)
            .HasForeignKey(c => c.InitialMessageId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne(c => c.Sender)
            .WithMany()
            .HasForeignKey(c => c.SenderId);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.DbConfigurations;

public class FriendConfiguration : IEntityTypeConfiguration<Friend>
{

    public void Configure(EntityTypeBuilder<Friend> builder)
    {
        builder.ToTable("UserFriends").HasKey(p => p.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

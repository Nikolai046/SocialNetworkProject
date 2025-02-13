using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.DbConfigurations;

public class FriendConfiguration : IEntityTypeConfiguration<Friend>
{

    public void Configure(EntityTypeBuilder<Friend> builder)
    {
        builder.ToTable("UserFriends");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).UseIdentityColumn();

        builder
            .HasOne(f => f.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        builder
            .HasOne(f => f.CurrentFriend)
            .WithMany()
            .HasForeignKey(f => f.CurrentFriendId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

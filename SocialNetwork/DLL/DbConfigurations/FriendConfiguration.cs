using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.DbConfigurations;

/// <summary>
/// Конфигурация сущности Friend для настройки маппинга в базе данных с использованием Entity Framework.
/// </summary>
public class FriendConfiguration : IEntityTypeConfiguration<Friend>
{
    /// <summary>
    /// Конфигурирует сущность Friend для использования в базе данных, определяя структуру таблицы, ключи и связи.
    /// </summary>
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
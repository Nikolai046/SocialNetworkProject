using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.DbConfigurations;

/// <summary>
/// Класс конфигурации сущности Comment для настройки маппинга модели к базе данных с использованием Entity Framework.
/// </summary>
public class CommentsConfiguration : IEntityTypeConfiguration<Comment>
{
    /// <summary>
    /// Конфигурирует сущность Comment для использования в базе данных, задавая параметры таблицы, ключей, связей и индексов.
    /// </summary>
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).UseIdentityColumn();

        builder
            .HasOne(c => c.Message)
            .WithMany(m => m.Comments)
            .HasForeignKey(c => c.InitialMessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(c => c.Sender)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(c => c.SenderId);

        builder.HasIndex(c => c.InitialMessageId);
    }
}
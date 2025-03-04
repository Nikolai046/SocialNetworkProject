using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.DbConfigurations;

/// <summary>
/// Конфигурация сущности ServiceData для настройки маппинга в базе данных с использованием Entity Framework.
/// </summary>
public class ServiceDataConfiguration : IEntityTypeConfiguration<ServiceData>
{
    /// <summary>
    /// Настраивает схему таблицы для сущности ServiceData в базе данных.
    /// Устанавливает имя таблицы, первичный ключ и уникальный индекс для свойства Key.
    /// </summary>
    public void Configure(EntityTypeBuilder<ServiceData> builder)
    {
        builder.ToTable("ServiceData");
        builder.HasKey(m => m.Key);
        builder.HasIndex(m => m.Key).IsUnique();
    }
}
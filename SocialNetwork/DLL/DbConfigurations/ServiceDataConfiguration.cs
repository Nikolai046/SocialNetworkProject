using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DLL.Entities;

namespace SocialNetwork.DLL.DbConfigurations;

public class ServiceDataConfiguration : IEntityTypeConfiguration<ServiceData>
{
    public void Configure(EntityTypeBuilder<ServiceData> builder)
    {
        builder.ToTable("ServiceData");
        builder.HasKey(m => m.Key);
        builder.HasIndex(m => m.Key).IsUnique();

    }
}
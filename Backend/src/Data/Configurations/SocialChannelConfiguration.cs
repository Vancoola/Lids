using Domain.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class SocialChannelConfiguration : IEntityTypeConfiguration<SocialChannel>
{
    public void Configure(EntityTypeBuilder<SocialChannel> builder)
    {
        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Id).ValueGeneratedOnAdd();

        builder.HasIndex(sc => new
        {
            sc.Platform,
            sc.ExternalId
        }).IsUnique();
    }
}
using Domain.Agents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class TelegramConnectionConfiguration : IEntityTypeConfiguration<TelegramConnection>
{
    public void Configure(EntityTypeBuilder<TelegramConnection> builder)
    {
        builder.HasIndex(c => c.UserId).IsUnique();
        builder.HasIndex(c => c.SessionKey).IsUnique();
    }
}

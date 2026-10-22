using Domain.Agents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class AgentConnectionConfiguration : IEntityTypeConfiguration<AgentConnection>
{
    public void Configure(EntityTypeBuilder<AgentConnection> builder)
    {
        builder.ToTable("AgentConnections");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.Platform);

        builder.HasDiscriminator<Platform>("PlatformDiscriminator")
            .HasValue<TelegramConnection>(Platform.Telegram);
    }
}
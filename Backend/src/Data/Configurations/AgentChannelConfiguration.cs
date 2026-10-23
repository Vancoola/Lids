using Domain.Agents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class AgentChannelConfiguration : IEntityTypeConfiguration<AgentChannel>
{
    public void Configure(EntityTypeBuilder<AgentChannel> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => new
        {
            x.AgentId,
            x.ChannelId
        }).IsUnique();

        builder.HasOne(a => a.Agent).WithMany(a => a.Channels).HasForeignKey(a => a.AgentId)
            .OnDelete(DeleteBehavior.ClientNoAction);
        builder.HasOne(a => a.Channel).WithMany().HasForeignKey(a => a.ChannelId);

        builder.ComplexProperty(a => a.Settings);
    }
}
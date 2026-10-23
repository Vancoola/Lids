using Domain.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(m => m.Text).IsRequired();

        builder.HasOne(m => m.Lead).WithMany(l => l.Messages).HasForeignKey(m => m.LeadId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(m => m.Agent).WithMany(a => a.Messages).HasForeignKey(m => m.AgentId)
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}

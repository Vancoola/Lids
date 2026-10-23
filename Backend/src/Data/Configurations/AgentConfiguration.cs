using Domain.Agents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Ignore(a => a.Events);

        builder.ComplexProperty(a => a.SendingSettings);

        builder.HasOne(a => a.Connection).WithMany().HasForeignKey(a => a.ConnectionId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        //builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

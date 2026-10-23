using Data.Identity;
using Domain.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.Name).IsRequired();

        builder
            .HasOne(l => l.ResponsibleAgent)
            .WithMany(a => a.Leads)
            .HasForeignKey(l => l.ResponsibleAgentId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(l => l.ResponsibleOperatorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
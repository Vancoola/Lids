using Data.Identity;
using Domain.Agents;
using Domain.Channels;
using Domain.Leads;
using Domain.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    
    internal DbSet<Lead> Leads => Set<Lead>();
    internal DbSet<Agent> Agents => Set<Agent>();
    internal DbSet<Message> Messages => Set<Message>();
    internal DbSet<SocialChannel> SocialChannels => Set<SocialChannel>();
    internal DbSet<AgentChannel> AgentChannels => Set<AgentChannel>();
    internal DbSet<AgentConnection> AgentConnections => Set<AgentConnection>();
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
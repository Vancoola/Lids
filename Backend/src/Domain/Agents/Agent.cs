using Domain.Abstractions;
using Domain.Common;
using Domain.Leads;
using Domain.Messages;
using Domain.Parsing;

namespace Domain.Agents;

public class Agent : AggregateRoot, ISoftDeletable
{

    private readonly List<Lead> _leads;
    private List<Message> _messages;
    private List<AgentChannel> _channels;
    
    #pragma warning disable CS8618
    private Agent() {}
    #pragma warning restore CS8618
    
    private Agent(int id, AgentConnection connection)
    {
        Id = id;
        Connection = connection;
        _messages = new List<Message>();
        _leads = new List<Lead>();
        _channels = new List<AgentChannel>();
        IsDeleted = false;
    }

    public int Id { get; private set; }

    public IReadOnlyCollection<Lead> Leads => _leads.AsReadOnly();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    public IReadOnlyCollection<AgentChannel> Channels => _channels.AsReadOnly();
    
    public AgentConnection Connection { get; private set; } = null!;
    public Platform Platform => Connection.Platform; 
    
    public bool IsDeleted { get; set; }

    public void ChangeChannelSettings(int agentChannelId, ParsingSettings settings, DateTimeOffset now)
    {
        var channel = Channels.Single(c => c.Id == agentChannelId);
        channel.ChangeSettings(settings, now);
        Raise(new ParsingScheduleChanged(agentChannelId));
    }

    public static Agent Create(int id, AgentConnection connection)
    {
        return new(id, connection);
    }
}

public record ParsingScheduleChanged(int AgentChannelId) : DomainEvent;

 
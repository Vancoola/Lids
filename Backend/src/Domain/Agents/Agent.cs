using Domain.Abstractions;
using Domain.Common;
using Domain.Exceptions;
using Domain.Leads;
using Domain.Messages;
using Domain.Parsing;
using Domain.Sending;

namespace Domain.Agents;

public class Agent : AggregateRoot, ISoftDeletable
{
    private readonly List<AgentChannel> _channels = new();

    private readonly List<Lead> _leads = new();
    private readonly List<Message> _messages = new();

#pragma warning disable CS8618
    private Agent()
    {
    }
#pragma warning restore CS8618

    private Agent(int id, AgentConnection connection, SendingSettings sendingSettings)
    {
        Id = id;
        Connection = connection;
        ConnectionId = connection.Id;
        SendingSettings = sendingSettings;
        IsDeleted = false;
    }

    public int Id { get; }

    public IReadOnlyCollection<Lead> Leads => _leads.AsReadOnly();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    public IReadOnlyCollection<AgentChannel> Channels => _channels.AsReadOnly();

    public Guid ConnectionId { get; private set; }
    public AgentConnection Connection { get; } = null!;
    public Platform Platform => Connection.Platform;

    public SendingSettings SendingSettings { get; private set; } = null!;

    public bool IsDeleted { get; set; }

    public void ChangeChannelSettings(int agentChannelId, ParsingSettings settings, DateTimeOffset now)
    {
        var channel = Channels.SingleOrDefault(c => c.Id == agentChannelId) ??
                      throw new AgentChannelNotFoundException(Id, agentChannelId);
        channel.ChangeSettings(settings, now);
        Raise(new ParsingScheduleChanged(agentChannelId));
    }

    public void ChangeSendingSettings(SendingSettings settings)
    {
        if (settings is null)
            throw new DomainValidationException("Sending settings are required.");

        SendingSettings = settings;
    }

    public static Agent Create(int id, AgentConnection connection, SendingSettings sendingSettings)
    {
        if (connection is null)
            throw new DomainValidationException("Agent connection is required.");

        if (sendingSettings is null)
            throw new DomainValidationException("Sending settings are required.");

        return new Agent(id, connection, sendingSettings);
    }
}

public record ParsingScheduleChanged(int AgentChannelId) : DomainEvent;
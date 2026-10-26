using Domain.Channels;
using Domain.Exceptions;
using Domain.Parsing;

namespace Domain.Agents;

public class AgentChannel
{
    private AgentChannel()
    {
    }

    private AgentChannel(int agentId, int channelId, ParsingSettings settings, DateTimeOffset now)
    {
        AgentId = agentId;
        ChannelId = channelId;
        IsEnabled = true;
        AccessStatus = AgentChannelAccessStatus.Unknown;
        ChangeSettings(settings, now);
    }

    public int Id { get; init; }

    public int AgentId { get; private set; }
    public Agent? Agent { get; private set; }
    public int ChannelId { get; private set; }
    public SocialChannel? Channel { get; private set; }


    public ParsingSettings Settings { get; private set; } = null!;


    public bool IsEnabled { get; private set; }

    public AgentChannelAccessStatus AccessStatus { get; private set; }

    public DateTimeOffset NextRunAt { get; private set; }
    public DateTimeOffset LastRunAt { get; private set; }

    public void ChangeSettings(ParsingSettings settings, DateTimeOffset now)
    {
        Settings = settings ?? throw new DomainValidationException("Parsing settings are required.");
        NextRunAt = now.Add(settings.Interval);
    }

    public void ChangeAccessStatus(AgentChannelAccessStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new DomainValidationException("Channel access status is invalid.");

        AccessStatus = status;
    }

    public void Enable()
    {
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    public static AgentChannel Create(int agentId, int channelId, ParsingSettings settings, DateTimeOffset now)
    {
        if (agentId <= 0)
            throw new DomainValidationException("Agent id must be greater than zero.");

        if (channelId <= 0)
            throw new DomainValidationException("Channel id must be greater than zero.");

        return new AgentChannel(agentId, channelId, settings, now);
    }
}
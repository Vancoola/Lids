using Domain.Agents;

namespace Domain.Channels;

public class SocialChannel
{
#pragma warning disable CS8618
    private SocialChannel()
    {
    }
#pragma warning restore CS8618

    public int Id { get; init; }

    public Platform Platform { get; private set; }

    public string ExternalId { get; private set; } = null!;

    public string? Title { get; private set; }

    public SocialChannelType Type { get; private set; }
}
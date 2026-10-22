using Domain.Channels;

namespace Domain.Agents;

public class AgentChannel
{
    
    public int Id { get; init; }
    
    public int AgentId { get; private set; }
    public Agent? Agent { get; private set; }
    public int ChannelId { get; private set; }
    public SocialChannel? Channel { get; private set; }
    
    public bool IsEnabled { get; private set; }
    
}
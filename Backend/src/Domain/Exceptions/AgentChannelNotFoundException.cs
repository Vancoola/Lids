using System.Net;

namespace Domain.Exceptions;

public sealed class AgentChannelNotFoundException(int agentId, int agentChannelId)
    : DomainException($"Channel assignment {agentChannelId} was not found for agent {agentId}.")
{
    public override HttpStatusCode Status => HttpStatusCode.NotFound;
}

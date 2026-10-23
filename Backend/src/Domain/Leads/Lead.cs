using Domain.Agents;
using Domain.Exceptions;
using Domain.Messages;

namespace Domain.Leads;

public class Lead
{
    private readonly List<Message> _messages = new List<Message>();

#pragma warning disable CS8618
    private Lead()
    {
    }
#pragma warning restore CS8618

    private Lead(string name, int responsibleAgentId)
    {
        Name = name;
        AssignAgent(responsibleAgentId);
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; init; }
    public string Name { get; private set; }

    public DateTime CreatedAt { get; init; }

    public Agent? ResponsibleAgent { get; private set; }
    public int ResponsibleAgentId { get; private set; }

    public Guid? ResponsibleOperatorId { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();


    public void AssignAgent(int agentId)
    {
        if (agentId <= 0)
            throw new DomainValidationException("Agent id must be greater than zero.");

        ResponsibleAgentId = agentId;
    }

    public void AssignOperator(Guid? operatorId)
    {
        if (operatorId == Guid.Empty)
            throw new DomainValidationException("Operator id must not be empty.");

        ResponsibleOperatorId = operatorId;
    }

    public static Lead Create(string name, int responsibleAgentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Lead name is required.");

        return new Lead(name, responsibleAgentId);
    }
}

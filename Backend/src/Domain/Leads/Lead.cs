using Domain.Agents;
using Domain.Messages;

namespace Domain.Leads;

public class Lead
{
    private readonly List<Message> _messages;

#pragma warning disable CS8618
    private Lead()
    {
    }
#pragma warning restore CS8618

    private Lead(string name, int responsibleAgentId)
    {
        Name = name;
        ResponsibleAgentId = responsibleAgentId;
        CreatedAt = DateTime.UtcNow;
        _messages = new List<Message>();
    }

    public int Id { get; init; }
    public string Name { get; private set; }

    public DateTime CreatedAt { get; init; }

    public Agent? ResponsibleAgent { get; private set; }
    public int ResponsibleAgentId { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();


    public static Lead Create(string name, int responsibleAgentId)
    {
        return new Lead(name, responsibleAgentId);
    }
}
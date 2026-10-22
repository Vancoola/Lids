using Domain.Agents;
using Domain.Leads;

namespace Domain.Messages;

public class Message
{
#pragma warning disable CS8618
    private Message()
    {
    }
#pragma warning restore CS8618

    private Message(
        int leadId,
        string text,
        MessageSenderType senderType,
        int agentId)
    {
        Id = Guid.CreateVersion7();
        LeadId = leadId;
        Text = text;
        SenderType = senderType;
        AgentId = agentId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public MessageSenderType SenderType { get; private set; }

    public Agent? Agent { get; private set; }
    public int AgentId { get; private set; }

    public Lead? Lead { get; private set; }
    public int LeadId { get; private set; }

    public static Message FromLead(int leadId, int agentId, string text)
    {
        return new Message(leadId, text, MessageSenderType.Lead, agentId);
    }

    public static Message FromAgent(
        int leadId,
        int agentId,
        string text)
    {
        return new Message(leadId, text, MessageSenderType.Agent, agentId);
    }
}
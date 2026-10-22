using Domain.Abstractions;
using Domain.Common;
using Domain.Leads;
using Domain.Messages;

namespace Domain.Agents;

public class Agent : AggregateRoot, ISoftDeletable
{

    private readonly List<Lead> _leads;
    private List<Message> _messages;
    
    #pragma warning disable CS8618
    private Agent() {}
    #pragma warning restore CS8618
    
    private Agent(int id, AgentConnection connection)
    {
        Id = id;
        Connection = connection;
        _messages = new List<Message>();
        _leads = new List<Lead>();
        IsDeleted = false;
    }

    public int Id { get; private set; }

    public IReadOnlyCollection<Lead> Leads => _leads;
    public IReadOnlyCollection<Message> Messages => _messages;
    
    public AgentConnection Connection { get; private set; } = null!;
    public Platform Platform => Connection.Platform;
    
    public bool IsDeleted { get; set; }
    

    public static Agent Create(int id, AgentConnection connection)
    {
        return new(id, connection);
    }
}

 
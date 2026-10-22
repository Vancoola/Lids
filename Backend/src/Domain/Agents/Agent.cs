using Domain.Abstractions;
using Domain.Leads;
using Domain.Messages;

namespace Domain.Agents;

public class Agent : ISoftDeletable
{

    private readonly List<Lead> _leads;
    private List<Message> _messages;
    
    #pragma warning disable CS8618
    private Agent() {}
    #pragma warning restore CS8618
    
    private Agent(int id, Platform platform)
    {
        Id = id;
        Platform = platform;
        _leads = new List<Lead>();
        IsDeleted = false;
    }

    public int Id { get; private set; }

    public Platform Platform { get; private set; }

    public IReadOnlyCollection<Lead> Leads => _leads;
    public IReadOnlyCollection<Message> Messages => _messages;
    
    public bool IsDeleted { get; set; }
    

    public static Agent Create(int id, Platform platform)
    {
        return new(id, platform);
    }
}
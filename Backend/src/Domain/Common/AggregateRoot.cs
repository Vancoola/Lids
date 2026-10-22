namespace Domain.Common;

public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _events = [];
    
    public IReadOnlyCollection<DomainEvent> Events => _events.AsReadOnly();
    
    protected void Raise(DomainEvent domainEvent) => _events.Add(domainEvent);
    
    public IReadOnlyCollection<DomainEvent> PullDomainEvents()
    {
        var domainEvents = _events.ToArray();
        _events.Clear();
        return domainEvents;
    }
}
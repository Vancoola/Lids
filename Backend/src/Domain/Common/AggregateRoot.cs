namespace Domain.Common;

public abstract class AggregateRoot
{
    private readonly List<DomainEvent> _events = [];
    
    /// <summary>
    /// Получить все события
    /// </summary>
    public IReadOnlyCollection<DomainEvent> Events => _events.AsReadOnly();
    
    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="domainEvent"></param>
    protected void Raise(DomainEvent domainEvent) => _events.Add(domainEvent);
    
    /// <summary>
    /// Собрать все события и очистить коллекцию
    /// </summary>
    /// <returns>Коллекция событий</returns>
    public IReadOnlyCollection<DomainEvent> PullDomainEvents()
    {
        var domainEvents = _events.ToArray();
        _events.Clear();
        return domainEvents;
    }
}
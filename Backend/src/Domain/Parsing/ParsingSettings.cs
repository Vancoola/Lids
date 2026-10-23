using Domain.Exceptions;

namespace Domain.Parsing;

public sealed record ParsingSettings
{
    public TimeSpan Interval { get; init; }

    public int MessagesPerRun { get; init; }

    public int MaxMessagesPerDay { get; init; }

    public bool ParseNewMessages { get; init; }

    public bool ParseHistory { get; init; }

    public static ParsingSettings Create(
        TimeSpan interval,
        int messagesPerRun,
        int maxMessagesPerDay,
        bool parseNewMessages,
        bool parseHistory)
    {
        if (interval <= TimeSpan.Zero)
            throw new DomainValidationException("Parsing interval must be greater than zero.");

        if (messagesPerRun <= 0)
            throw new DomainValidationException("Parsing message limit per run must be greater than zero.");

        if (maxMessagesPerDay <= 0)
            throw new DomainValidationException("Daily parsing message limit must be greater than zero.");

        return new ParsingSettings
        {
            Interval = interval,
            MessagesPerRun = messagesPerRun,
            MaxMessagesPerDay = maxMessagesPerDay,
            ParseNewMessages = parseNewMessages,
            ParseHistory = parseHistory
        };
    }
}
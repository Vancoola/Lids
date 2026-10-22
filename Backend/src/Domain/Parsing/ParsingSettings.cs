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
            throw new ArgumentOutOfRangeException(nameof(interval));

        if (messagesPerRun <= 0 || maxMessagesPerDay <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(messagesPerRun));

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
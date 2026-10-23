using Domain.Exceptions;

namespace Domain.Sending;

public sealed record SendingSettings
{
    public TimeSpan Interval { get; init; }

    // Нулевой лимит запрещает отправку сообщений.
    public int MaxMessagesPerDay { get; init; }

    public static SendingSettings Create(TimeSpan interval, int maxMessagesPerDay)
    {
        if (interval < TimeSpan.Zero)
            throw new DomainValidationException("Sending interval must not be negative.");

        if (maxMessagesPerDay < 0)
            throw new DomainValidationException("Daily sending message limit must not be negative.");

        return new SendingSettings
        {
            Interval = interval,
            MaxMessagesPerDay = maxMessagesPerDay
        };
    }
}

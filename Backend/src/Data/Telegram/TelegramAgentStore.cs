using Business.Abstractions.Telegram;
using Business.Exceptions;
using Domain.Agents;
using Domain.Sending;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Data.Telegram;

public sealed class TelegramAgentStore(IDbContextFactory<ApplicationDbContext> contextFactory) : ITelegramAgentStore
{
    public async Task<int> CreateAsync(Guid attemptId, long userId, byte[] sessionData, CancellationToken ct)
    {
        if (userId <= 0 || sessionData.Length == 0)
            throw new TelegramAuthorizationException("An authorized Telegram account and session are required.");

        await using var context = await contextFactory.CreateDbContextAsync(ct);
        var sessionKey = attemptId.ToString("N");
        var existingId = await FindAgentAsync(context, sessionKey, ct);
        if (existingId is not null)
            return existingId.Value;

        if (await context.Set<TelegramConnection>().AnyAsync(c => c.UserId == userId, ct))
            throw new TelegramAccountAlreadyConnectedException();

        var connection = TelegramConnection.Create(sessionKey, userId);
        var agent = Agent.Create(0, connection, SendingSettings.Create(TimeSpan.Zero, 0));
        context.Add(TelegramSession.Create(sessionKey, sessionData));
        context.Add(agent);

        try
        {
            await context.SaveChangesAsync(ct);
            return agent.Id;
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "IX_AgentConnections_UserId" or "IX_AgentConnections_SessionKey"
                or "PK_TelegramSessions"
            })
        {
            context.ChangeTracker.Clear();
            existingId = await FindAgentAsync(context, sessionKey, ct);
            if (existingId is not null)
                return existingId.Value;

            throw new TelegramAccountAlreadyConnectedException();
        }
    }

    private static Task<int?> FindAgentAsync(ApplicationDbContext context, string sessionKey, CancellationToken ct)
    {
        return context.Set<Agent>()
            .Where(a => a.Connection is TelegramConnection &&
                        ((TelegramConnection)a.Connection).SessionKey == sessionKey)
            .Select(a => (int?)a.Id)
            .SingleOrDefaultAsync(ct);
    }
}
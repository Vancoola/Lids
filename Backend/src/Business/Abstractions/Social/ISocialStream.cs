using System.Threading.Channels;
using Business.Contracts.Messaging;

namespace Business.Abstractions.Social;

public interface ISocialStream : IAsyncDisposable
{
    Task RunAsync(
        ChannelWriter<IncomingMessage> writer,
        CancellationToken ct);
}
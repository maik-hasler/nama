namespace Nama.Test.Shared;

internal sealed class PingRequestHandler
    : IRequestHandler<PingRequest, int>
{
    public Task<int> Handle(
        PingRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(42);
    }
}
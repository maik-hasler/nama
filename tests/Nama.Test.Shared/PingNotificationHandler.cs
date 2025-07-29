namespace Nama.Test.Shared;

internal sealed class PingNotificationHandler
    : INotificationHandler<PingNotification>
{
    public Task Handle(
        PingNotification notification,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
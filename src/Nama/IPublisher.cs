namespace Nama;

public interface IPublisher
{
    Task Publish(
        INotification notification,
        CancellationToken cancellationToken);
}
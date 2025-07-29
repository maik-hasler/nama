using Microsoft.Extensions.DependencyInjection;
using Nama.Test.Shared;
using Xunit;

namespace Nama.Test;

public sealed class AssemblyResolutionTests
{
    private readonly IServiceProvider _serviceProvider;
    
    public AssemblyResolutionTests()
    {
        var services = new ServiceCollection();
        services.AddNama(typeof(PingNotification).Assembly);
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [Fact]
    public void ServiceProvider_ShouldResolveSender_AfterServiceRegistration()
    {
        Assert.NotNull(_serviceProvider.GetRequiredService<ISender>());
    }
    
    [Fact]
    public void ServiceProvider_ShouldResolvePublisher_AfterServiceRegistration()
    {
        Assert.NotNull(_serviceProvider.GetRequiredService<IPublisher>());
    }
    
    [Fact]
    public void ServiceProvider_ShouldResolveNotificationHandler_AfterServiceRegistration()
    {
        Assert.NotNull(_serviceProvider.GetRequiredService<INotificationHandler<PingNotification>>());
    }
}
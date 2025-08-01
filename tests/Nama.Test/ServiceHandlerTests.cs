using Microsoft.Extensions.DependencyInjection;
using Nama.Test.Shared;
using Xunit;

namespace Nama.Test;

public sealed class ServiceHandlerTests
{
    private readonly IServiceProvider _serviceProvider;
    
    public ServiceHandlerTests()
    {
        var services = new ServiceCollection();
        services.AddNama(typeof(PingNotification).Assembly);
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [Fact]
    public async Task Handler_ShouldProcessRequest_WhenHandlerIsAvailable()
    {
        var sender = _serviceProvider.GetRequiredService<ISender>();
        
        var result = await sender.Send(new PingRequest(), TestContext.Current.CancellationToken);
        
        Assert.Equal(42, result);
    }
}
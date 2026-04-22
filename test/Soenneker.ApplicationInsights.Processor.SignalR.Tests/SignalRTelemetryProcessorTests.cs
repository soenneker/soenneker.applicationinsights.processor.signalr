using Soenneker.Tests.HostedUnit;

namespace Soenneker.ApplicationInsights.Processor.SignalR.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class SignalRTelemetryProcessorTests : HostedUnitTest
{
    public SignalRTelemetryProcessorTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}

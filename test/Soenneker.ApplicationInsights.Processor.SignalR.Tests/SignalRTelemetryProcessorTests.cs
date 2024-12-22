using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.ApplicationInsights.Processor.SignalR.Tests;

[Collection("Collection")]
public class SignalRTelemetryProcessorTests : FixturedUnitTest
{
    public SignalRTelemetryProcessorTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}

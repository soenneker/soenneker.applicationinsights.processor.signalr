using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;

namespace Soenneker.ApplicationInsights.Processor.SignalR.Tests;

public sealed class SignalRTelemetryProcessorTests
{
    [Test]
    public async ValueTask Default_preserves_existing_filter_and_negotiation()
    {
        var options = new AspNetCoreTraceInstrumentationOptions { Filter = ctx => ctx.Request.Path != "/excluded" };
        var previous = options.Filter;
        new SignalRTelemetryProcessor().Configure(options);
        await Assert.That(ReferenceEquals(previous, options.Filter)).IsTrue();
        await Assert.That(options.Filter!(Context("/hubs/orders/negotiate"))).IsTrue();
        await Assert.That(options.Filter(Context("/excluded"))).IsFalse();
    }

    [Test]
    public async ValueTask Suppression_composes_filter_preserves_negotiation_and_matches_segments()
    {
        var settings = new SignalRTelemetryOptions { SuppressHubRequests = true, HubPathPrefixes = [new("/realtime")] };
        var processor = new SignalRTelemetryProcessor(Options.Create(settings));
        settings.HubPathPrefixes[0] = new("/changed");
        var options = new AspNetCoreTraceInstrumentationOptions { Filter = ctx => ctx.Request.Path != "/realtime/secret/negotiate" };
        processor.Configure(options);
        await Assert.That(options.Filter!(Context("/realtime/orders"))).IsFalse();
        await Assert.That(options.Filter(Context("/realtime/orders/negotiate"))).IsTrue();
        await Assert.That(options.Filter(Context("/realtime/secret/negotiate"))).IsFalse();
        await Assert.That(options.Filter(Context("/realtime-other/orders"))).IsTrue();
        await Assert.That(options.Filter(Context("/hubs/orders"))).IsTrue();
    }

    [Test]
    public async ValueTask Refresh_traces_are_preserved_unless_disabled_and_respect_existing_filter()
    {
        var settings = new SignalRTelemetryOptions { SuppressHubRequests = true, PreserveNegotiation = false };
        var options = new AspNetCoreTraceInstrumentationOptions { Filter = ctx => ctx.Request.Path != "/hubs/secret/refresh" };
        new SignalRTelemetryProcessor(Options.Create(settings)).Configure(options);
        await Assert.That(options.Filter!(Context("/hubs/orders/refresh"))).IsTrue();
        await Assert.That(options.Filter(Context("/hubs/orders/REFRESH/"))).IsTrue();
        await Assert.That(options.Filter(Context("/hubs/secret/refresh"))).IsFalse();
        await Assert.That(options.Filter(Context("/hubs/orders/negotiate"))).IsFalse();
        await Assert.That(options.Filter(Context("/hubs/orders/refreshing"))).IsFalse();
        settings.PreserveAuthenticationRefresh = false;
        var suppressed = new AspNetCoreTraceInstrumentationOptions();
        new SignalRTelemetryProcessor(Options.Create(settings)).Configure(suppressed);
        await Assert.That(suppressed.Filter!(Context("/hubs/orders/refresh"))).IsFalse();
    }

    private static HttpContext Context(string path)
    {
        var context = new DefaultHttpContext(); context.Request.Path = path; return context;
    }
}

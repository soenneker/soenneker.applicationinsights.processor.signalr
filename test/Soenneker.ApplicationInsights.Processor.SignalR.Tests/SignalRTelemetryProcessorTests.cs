using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Soenneker.ApplicationInsights.Processor.SignalR.Tests;

public sealed class SignalRTelemetryProcessorTests
{
    [Test]
    [Arguments("/realtime/orders", false)]
    [Arguments("/realtime/orders/negotiate", true)]
    [Arguments("/realtime/orders/REFRESH/", true)]
    [Arguments("/realtime-other/orders", true)]
    public async ValueTask Native_activity_filter_matches_segments_and_preserves_control_requests(string path, bool recorded)
    {
        var settings = new SignalRTelemetryOptions { SuppressHubRequests = true, HubPathPrefixes = [new("/realtime")] };
        var processor = new SignalRTelemetryProcessor(Options.Create(settings));
        settings.HubPathPrefixes[0] = new("/changed");
        using var services = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(services);
        var called = false;
        processor.Configure(builder => builder.Run(_ => { called = true; return Task.CompletedTask; }))(app);
        using var activity = new Activity("request");
        activity.ActivityTraceFlags = ActivityTraceFlags.Recorded;
        activity.IsAllDataRequested = true;
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Features.Set<IHttpActivityFeature>(new TestActivityFeature(activity));
        await app.Build()(context);
        await Assert.That(activity.Recorded).IsEqualTo(recorded);
        await Assert.That(activity.IsAllDataRequested).IsEqualTo(recorded);
        await Assert.That(called).IsTrue();
    }

    [Test]
    public async ValueTask Default_does_not_resample_an_unrecorded_activity()
    {
        using var services = new ServiceCollection().BuildServiceProvider();
        var app = new ApplicationBuilder(services);
        new SignalRTelemetryProcessor().Configure(builder => builder.Run(_ => Task.CompletedTask))(app);
        using var activity = new Activity("request") { IsAllDataRequested = false };
        var context = new DefaultHttpContext();
        context.Request.Path = "/hubs/orders";
        context.Features.Set<IHttpActivityFeature>(new TestActivityFeature(activity));
        await app.Build()(context);
        await Assert.That(activity.Recorded).IsFalse();
        await Assert.That(activity.IsAllDataRequested).IsFalse();
    }
}

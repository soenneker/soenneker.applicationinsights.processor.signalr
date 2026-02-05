using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;

namespace Soenneker.ApplicationInsights.Processor.SignalR;

/// <summary>
/// A telemetry processor connecting SignalR hub traffic and Application Insights
/// </summary>
public sealed class SignalRTelemetryProcessor : IConfigureOptions<AspNetCoreTraceInstrumentationOptions>
{
    public void Configure(AspNetCoreTraceInstrumentationOptions options)
    {
        // Filter receives HttpContext; returning false prevents collection. :contentReference[oaicite:1]{index=1}
        options.Filter = static (HttpContext ctx) =>
        {
            PathString path = ctx.Request.Path;

            // Covers /hubs, /hubs/anything, and typical negotiate endpoints (/hubs/foo/negotiate)
            return !path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase);
        };
    }
}
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;

namespace Soenneker.ApplicationInsights.Processor.SignalR;

public sealed class SignalRTelemetryProcessor : IStartupFilter
{
    private readonly bool _suppress;
    private readonly bool _preserveNegotiation;
    private readonly bool _preserveAuthenticationRefresh;
    private readonly PathString[] _prefixes;

    public SignalRTelemetryProcessor() : this(Options.Create(new SignalRTelemetryOptions())) { }

    public SignalRTelemetryProcessor(IOptions<SignalRTelemetryOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        SignalRTelemetryOptions value = options.Value;
        _suppress = value.SuppressHubRequests;
        _preserveNegotiation = value.PreserveNegotiation;
        _preserveAuthenticationRefresh = value.PreserveAuthenticationRefresh;
        _prefixes = (PathString[])(value.HubPathPrefixes ?? throw new ArgumentException("Hub path prefixes are required.")).Clone();
        if (_prefixes.Any(prefix => !prefix.HasValue || prefix.Value == "/"))
            throw new ArgumentException("Hub prefixes must be non-root paths.");
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        ArgumentNullException.ThrowIfNull(next);
        return app =>
        {
            app.Use(async (context, continuation) =>
            {
                if (ShouldSuppress(context) && context.Features.Get<IHttpActivityFeature>()?.Activity is { } activity)
                {
                    activity.IsAllDataRequested = false;
                    activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
                }
                await continuation(context);
            });
            next(app);
        };
    }

    private bool ShouldSuppress(HttpContext context)
    {
        if (!_suppress) return false;
        PathString path = context.Request.Path;
        if (!_prefixes.Any(prefix => path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))) return false;
        string? endpoint = path.Value?.TrimEnd('/');
        return !((_preserveNegotiation && endpoint?.EndsWith("/negotiate", StringComparison.OrdinalIgnoreCase) == true) ||
            (_preserveAuthenticationRefresh && endpoint?.EndsWith("/refresh", StringComparison.OrdinalIgnoreCase) == true));
    }
}

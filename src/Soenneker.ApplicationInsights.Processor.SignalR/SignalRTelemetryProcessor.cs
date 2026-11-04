using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;

namespace Soenneker.ApplicationInsights.Processor.SignalR;

/// <summary>Composes optional hub request filtering with the existing ASP.NET Core trace filter.</summary>
public sealed class SignalRTelemetryProcessor : IConfigureOptions<AspNetCoreTraceInstrumentationOptions>
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

    public void Configure(AspNetCoreTraceInstrumentationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (!_suppress) return;
        Func<HttpContext, bool>? previous = options.Filter;
        options.Filter = context =>
        {
            if (previous != null && !previous(context)) return false;
            PathString path = context.Request.Path;
            if (!_prefixes.Any(prefix => path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase))) return true;
            string? endpoint = path.Value?.TrimEnd('/');
            return (_preserveNegotiation && endpoint?.EndsWith("/negotiate", StringComparison.OrdinalIgnoreCase) == true) ||
                (_preserveAuthenticationRefresh && endpoint?.EndsWith("/refresh", StringComparison.OrdinalIgnoreCase) == true);
        };
    }
}

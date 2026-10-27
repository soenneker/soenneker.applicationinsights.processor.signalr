using Microsoft.AspNetCore.Http;

namespace Soenneker.ApplicationInsights.Processor.SignalR;

/// <summary>Controls optional exclusion of hub transport requests from request tracing.</summary>
public sealed class SignalRTelemetryOptions
{
    /// <summary>Opt in to dropping transport requests, including their failures. Default false preserves diagnostic traces.</summary>
    public bool SuppressHubRequests { get; set; }
    /// <summary>Keep negotiation traces when transport suppression is enabled. Default true.</summary>
    public bool PreserveNegotiation { get; set; } = true;
    /// <summary>Hub route prefixes to match by complete path segment. Default is /hubs.</summary>
    public PathString[] HubPathPrefixes { get; set; } = [new("/hubs")];
}

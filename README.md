[![](https://img.shields.io/nuget/v/soenneker.applicationinsights.processor.signalr.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.applicationinsights.processor.signalr/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.applicationinsights.processor.signalr/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.applicationinsights.processor.signalr/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.applicationinsights.processor.signalr.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.applicationinsights.processor.signalr/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.applicationinsights.processor.signalr/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.applicationinsights.processor.signalr/actions/workflows/codeql.yml)

# Soenneker.ApplicationInsights.Processor.SignalR

Configurable OpenTelemetry filtering for SignalR hub traffic. Request tracing is preserved by default, including negotiation and failures.

## Registration

```csharp
using Soenneker.ApplicationInsights.Processor.SignalR.Registrars;

builder.Services.AddSignalRHubTelemetryProcessor(options =>
{
    options.SuppressHubRequests = true; // Explicitly opt in to dropping transport requests.
    options.PreserveNegotiation = true;
    options.HubPathPrefixes = [new("/hubs"), new("/realtime")];
});
```

Configure ASP.NET Core OpenTelemetry instrumentation/Application Insights in the host as usual. Calling the parameterless registrar leaves existing request tracing intact. This differs from the earlier default that excluded all `/hubs` traffic.

When enabled, the processor combines its predicate with the filter already configured. Register other assigning configurators before this one; a later configurator that replaces `Filter` can still override the composed predicate. Negotiation remains subject to the earlier filter. Route prefixes are matched case-insensitively by whole path segment; `/hubs-other` does not match `/hubs`.

Suppression occurs when collection starts, before the response status is known. Enabling it drops transport failures too. Keep it disabled when those traces are needed. `PreserveNegotiation = false` also suppresses negotiation. The filter affects request traces, not application logs or independently emitted telemetry. Path matching applies to non-SignalR endpoints under the configured prefixes as well.

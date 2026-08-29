[![](https://img.shields.io/nuget/v/soenneker.applicationinsights.processor.signalr.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.applicationinsights.processor.signalr/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.applicationinsights.processor.signalr/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.applicationinsights.processor.signalr/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.applicationinsights.processor.signalr.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.applicationinsights.processor.signalr/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.applicationinsights.processor.signalr/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.applicationinsights.processor.signalr/actions/workflows/codeql.yml)

# Soenneker.ApplicationInsights.Processor.SignalR

An OpenTelemetry ASP.NET Core instrumentation filter that excludes requests under `/hubs` from exported request traces.

Use it when SignalR connection, transport, and negotiate requests create unwanted Application Insights volume and every application hub is mapped beneath `/hubs`.

## Installation

```bash
dotnet add package Soenneker.ApplicationInsights.Processor.SignalR
```

## Registration

```csharp
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Soenneker.ApplicationInsights.Processor.SignalR.Registrars;

builder.Services
       .AddOpenTelemetry()
       .UseAzureMonitor();

builder.Services.AddSignalRHubTelemetryProcessor();
```

The registrar adds `SignalRTelemetryProcessor` as an `IConfigureOptions<AspNetCoreTraceInstrumentationOptions>` implementation. It does not register a separately resolved processor service.

## What is filtered

The filter performs a case-insensitive `PathString.StartsWithSegments("/hubs")` check.

Filtered examples:

```text
/hubs
/hubs/chat
/hubs/chat/negotiate
/HUBS/notifications
```

Requests outside that path segment continue through ASP.NET Core instrumentation:

```text
/api/hubs
/hub
/health
```

The filter prevents collection of the matching ASP.NET Core server activities. It does not merely hide them in the Azure portal, and it does not filter application logs or custom telemetry emitted separately.

## Map hubs consistently

The package assumes hubs are rooted at `/hubs`:

```csharp
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationsHub>("/hubs/notifications");
```

A hub mapped elsewhere is not filtered. Conversely, every non-SignalR endpoint under `/hubs` is also filtered.

## Composition caveat

`SignalRTelemetryProcessor` assigns `AspNetCoreTraceInstrumentationOptions.Filter`. It does not combine its predicate with a filter already configured by the application or another package. Whichever options configurator assigns the property last determines the effective filter.

When the application needs several conditions, prefer one application-owned predicate that composes them explicitly:

```csharp
builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
{
    options.Filter = context =>
        !context.Request.Path.StartsWithSegments("/hubs") &&
        !context.Request.Path.StartsWithSegments("/health");
});
```

Do not register the package's configurator in addition to a composed application filter unless the resulting options order is intentional.

## API

`AddSignalRHubTelemetryProcessor(IServiceCollection)` registers the ASP.NET Core trace-options configurator and returns the same service collection for chaining.

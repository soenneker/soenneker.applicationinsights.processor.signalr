[![](https://img.shields.io/nuget/v/soenneker.applicationinsights.processor.signalr.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.applicationinsights.processor.signalr/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.applicationinsights.processor.signalr/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.applicationinsights.processor.signalr/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.applicationinsights.processor.signalr.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.applicationinsights.processor.signalr/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.applicationinsights.processor.signalr/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.applicationinsights.processor.signalr/actions/workflows/codeql.yml)

# Soenneker.ApplicationInsights.Processor.SignalR

Represents the signal r telemetry processor registrar.

## Install

```bash
dotnet add package Soenneker.ApplicationInsights.Processor.SignalR
```

## Quick start

```csharp
using Soenneker.ApplicationInsights.Processor.SignalR.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddSignalRHubTelemetryProcessor();
```

Adds signal r hub telemetry processor.

## What you get

- `SignalRTelemetryProcessorRegistrar` — Represents the signal r telemetry processor registrar.
- `SignalRTelemetryProcessor` — A telemetry processor connecting SignalR hub traffic and Application Insights.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `SignalRTelemetryProcessorRegistrar.AddSignalRHubTelemetryProcessor(services)` | Adds signal r hub telemetry processor. | The same service collection, so additional registrations can be chained. |
| `SignalRTelemetryProcessor.Configure(options)` | Applies signal r telemetry processor-specific settings to the supplied options. | Returns no value; the requested change is complete when the method returns. |

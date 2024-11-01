using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Soenneker.ApplicationInsights.Processor.SignalR;

/// <summary>
/// A telemetry processor connecting SignalR hub traffic and Application Insights
/// </summary>
public class SignalRTelemetryProcessor : ITelemetryProcessor
{
    private ITelemetryProcessor Next { get; }

    public SignalRTelemetryProcessor(ITelemetryProcessor next)
    {
        Next = next;
    }

    public void Process(ITelemetry item)
    {
        if (!ShouldSend(item))
            return;

        Next.Process(item);
    }

    private static bool ShouldSend(ITelemetry item)
    {
        if (item is not RequestTelemetry telemetry)
            return true;

        if (telemetry.Url == null)
            return false;

        return !telemetry.Url.AbsolutePath.Contains("/hubs");
    }
}
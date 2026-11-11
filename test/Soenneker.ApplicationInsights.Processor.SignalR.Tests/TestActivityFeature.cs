using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace Soenneker.ApplicationInsights.Processor.SignalR.Tests;

internal sealed class TestActivityFeature(Activity activity) : IHttpActivityFeature
{
    public Activity Activity { get; set; } = activity;
}

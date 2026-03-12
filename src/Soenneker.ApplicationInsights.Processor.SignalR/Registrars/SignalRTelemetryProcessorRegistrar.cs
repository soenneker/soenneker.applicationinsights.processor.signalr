using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Soenneker.ApplicationInsights.Processor.SignalR.Registrars
{
    public static class SignalRTelemetryProcessorRegistrar
    {
        public static IServiceCollection AddSignalRHubTelemetryProcessor(this IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<AspNetCoreTraceInstrumentationOptions>, SignalRTelemetryProcessor>();
            return services;
        }
    }
}

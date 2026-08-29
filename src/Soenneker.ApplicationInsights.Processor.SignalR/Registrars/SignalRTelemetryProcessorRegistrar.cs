using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Instrumentation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Soenneker.ApplicationInsights.Processor.SignalR.Registrars
{
    /// <summary>
    /// Represents the signal r telemetry processor registrar.
    /// </summary>
    public static class SignalRTelemetryProcessorRegistrar
    {
        /// <summary>
        /// Adds signal r hub telemetry processor.
        /// </summary>
        /// <param name="services">Service collection that receives the registration.</param>
        /// <returns>The same service collection, so additional registrations can be chained.</returns>
        public static IServiceCollection AddSignalRHubTelemetryProcessor(this IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<AspNetCoreTraceInstrumentationOptions>, SignalRTelemetryProcessor>();
            return services;
        }
    }
}

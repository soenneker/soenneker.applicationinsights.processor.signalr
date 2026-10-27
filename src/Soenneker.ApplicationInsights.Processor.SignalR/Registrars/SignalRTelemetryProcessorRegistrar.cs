using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Instrumentation.AspNetCore;
using System;

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
        public static IServiceCollection AddSignalRHubTelemetryProcessor(this IServiceCollection services) => AddSignalRHubTelemetryProcessor(services, null);

        /// <summary>Registers optional hub transport suppression while retaining the previously configured request filter.</summary>
        public static IServiceCollection AddSignalRHubTelemetryProcessor(this IServiceCollection services, Action<SignalRTelemetryOptions>? configure)
        {
            services.AddOptions<SignalRTelemetryOptions>();
            if (configure != null) services.Configure(configure);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<AspNetCoreTraceInstrumentationOptions>, SignalRTelemetryProcessor>());
            return services;
        }
    }
}

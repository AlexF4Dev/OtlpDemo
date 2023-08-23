using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ServiceDiscovery;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class Extensions
{
    public static void AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName);

        builder.Services.AddServiceDiscovery().AddConfigurationServiceEndPointResolver();

        HttpClientFactoryServiceCollectionExtensions.ConfigureHttpClientDefaults(builder.Services,
            b => b.AddServiceDiscovery().AddStandardResilienceHandler());

        builder.Logging.AddOpenTelemetry(builder =>
        {
            builder.SetResourceBuilder(resourceBuilder);
            builder.AddOtlpExporter();
        });

        builder.Services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder.SetResourceBuilder(resourceBuilder);

                    builder.AddOtlpExporter();

                    builder.AddSource("Microsoft.AspNetCore", "System.Net.Http");
                })
                .WithMetrics(builder =>
                {
                    builder.SetResourceBuilder(resourceBuilder);

                    builder.AddOtlpExporter();

                    builder.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "System.Net.Http");
                    builder.AddView("request-duration",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
                        });
                });
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenAdm.IoC;

public static class DependencyInjectTelemetry
{
    public static IServiceCollection AddTelemetria(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration["TelemetriaAtiva"]?.ToUpper() != "TRUE")
        {
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService("OpenAdm.Api"))
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(configuration["TelemetriaUrl"]!);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            });

        return services;
    }
}
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenAdm.IoC;

public static class DependencyInjectTelemetry
{
    private static readonly ActivitySource MemorySource = new("OpenAdm.Memory");

    public static IServiceCollection AddTelemetria(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("TelemetriaAtiva"))
            return services;

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService("OpenAdm.Api"))
            .WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new AlwaysOnSampler())
                    .AddSource("OpenAdm.Memory")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(configuration["TelemetriaUrl"]!);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            });

        services.AddHostedService<MemoryMonitor>();

        return services;
    }

    private sealed class MemoryMonitor : BackgroundService
    {
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            CollectMemory();

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                CollectMemory();
            }
        }

        private static void CollectMemory()
        {
            using var activity = MemorySource.StartActivity(
                "memory.snapshot",
                ActivityKind.Internal);

            if (activity is null)
                return;

            using var process = Process.GetCurrentProcess();
            process.Refresh();

            var gc = GC.GetGCMemoryInfo();

            const double mb = 1024d * 1024d;

            activity.SetTag(
                "memory.working_set_mb",
                Math.Round(process.WorkingSet64 / mb, 2));

            activity.SetTag(
                "memory.managed_mb",
                Math.Round(GC.GetTotalMemory(false) / mb, 2));

            activity.SetTag(
                "memory.private_mb",
                Math.Round(process.PrivateMemorySize64 / mb, 2));

            activity.SetTag(
                "memory.gc_heap_mb",
                Math.Round(gc.HeapSizeBytes / mb, 2));

            activity.SetTag(
                "memory.gen0_collections",
                GC.CollectionCount(0));

            activity.SetTag(
                "memory.gen1_collections",
                GC.CollectionCount(1));

            activity.SetTag(
                "memory.gen2_collections",
                GC.CollectionCount(2));

            activity.SetTag(
                "memory.thread_count",
                process.Threads.Count);
        }
    }
}

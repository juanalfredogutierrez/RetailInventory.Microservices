using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace BuildingBlocks.Observability;

public static class SerilogExtensions
{
    public static ConfigureHostBuilder AddSerilogLogging(
        this ConfigureHostBuilder host,
        WebApplicationBuilder builder)
    {
        host.UseSerilog((context, services, loggerConfiguration) =>
        {
            var version =
                typeof(SerilogExtensions)
                    .Assembly
                    .GetName()
                    .Version?
                    .ToString();

            var seqUrl =
                builder.Configuration["Serilog:SeqUrl"];

            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)

                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()

                .Enrich.WithProperty(
                    "Application",
                    builder.Environment.ApplicationName)

                .Enrich.WithProperty(
                    "ServiceName",
                    builder.Environment.ApplicationName)

                .Enrich.WithProperty(
                    "Environment",
                    builder.Environment.EnvironmentName)

                .Enrich.WithProperty(
                    "Version",
                    version)

                .MinimumLevel.Information()

                .MinimumLevel.Override(
                    "Microsoft",
                    LogEventLevel.Warning)

                .MinimumLevel.Override(
                    "Microsoft.AspNetCore",
                    LogEventLevel.Warning)

                .MinimumLevel.Override(
                    "System",
                    LogEventLevel.Warning)

                .MinimumLevel.Override(
                "LuckyPennySoftware",
                LogEventLevel.Error)
                .WriteTo.Console();

            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                loggerConfiguration.WriteTo.Seq(seqUrl);
            }
        });

        return host;
    }
}
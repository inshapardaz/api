using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Inshapardaz.Api.Infrastructure;

/// <summary>
/// Exports traces, metrics and logs over OTLP (to the Aspire dashboard in dev and production).
/// Everything is driven by the standard OTEL_EXPORTER_OTLP_ENDPOINT / OTEL_EXPORTER_OTLP_HEADERS /
/// OTEL_EXPORTER_OTLP_PROTOCOL / OTEL_SERVICE_NAME variables and is a no-op when no endpoint is set.
/// </summary>
public static class OpenTelemetrySetup
{
    private const string EndpointVariable = "OTEL_EXPORTER_OTLP_ENDPOINT";

    public static bool IsEnabled => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EndpointVariable));

    public static IServiceCollection AddApiTelemetry(this IServiceCollection services, string serviceName)
    {
        if (!IsEnabled)
        {
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter())
            .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation().AddOtlpExporter());

        return services;
    }

    public static LoggerConfiguration WriteToOpenTelemetry(this LoggerConfiguration config, string serviceName)
    {
        if (!IsEnabled)
        {
            return config;
        }

        var endpoint = Environment.GetEnvironmentVariable(EndpointVariable)!;
        var protocol = string.Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), "http/protobuf", StringComparison.OrdinalIgnoreCase)
            ? OtlpProtocol.HttpProtobuf
            : OtlpProtocol.Grpc;
        var headers = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");

        return config.WriteTo.OpenTelemetry(o =>
        {
            // The sink appends /v1/logs itself for HTTP; for gRPC the base endpoint is used as-is.
            o.Endpoint = protocol == OtlpProtocol.HttpProtobuf ? $"{endpoint.TrimEnd('/')}/v1/logs" : endpoint;
            o.Protocol = protocol;
            o.ResourceAttributes = new Dictionary<string, object> { ["service.name"] = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? serviceName };

            foreach (var header in (headers ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var separator = header.IndexOf('=');
                if (separator > 0)
                {
                    o.Headers[header[..separator]] = header[(separator + 1)..];
                }
            }
        });
    }
}

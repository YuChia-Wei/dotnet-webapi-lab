using System.Diagnostics.Metrics;

namespace dotnet.WebApi.Observability.Metrics;

/// <summary>
/// https://learn.microsoft.com/zh-tw/dotnet/core/diagnostics/compare-metric-apis#systemdiagnosticsmetrics
/// </summary>
public static class ApplicationMeter
{
    public static readonly Meter Meter = new(DiagnosticsResource.Name());
}
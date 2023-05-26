namespace dotnet.WebApi.Observability.Tracing;

public static class OpenTelemetryActivitySource
{
    public static readonly System.Diagnostics.ActivitySource RegisteredActivity =
        new(DiagnosticsResource.Name());
}
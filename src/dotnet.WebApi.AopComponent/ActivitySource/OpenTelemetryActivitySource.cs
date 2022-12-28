namespace dotnet.WebApi.AopComponent.ActivitySource;

public static class OpenTelemetryActivitySource
{
    public static System.Diagnostics.ActivitySource ActiveSource =
        new System.Diagnostics.ActivitySource(Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ??
                                              AppDomain.CurrentDomain.FriendlyName);
}
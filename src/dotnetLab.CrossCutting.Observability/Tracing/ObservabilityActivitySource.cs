namespace dotnetLab.CrossCutting.Observability.Tracing;

public static class ObservabilityActivitySource
{
    public static readonly System.Diagnostics.ActivitySource RegisteredActivity =
        new(DiagnosticsResource.LabObservabilityLibrary);
}
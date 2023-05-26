using AspectInjector.Broker;

namespace dotnet.WebApi.Observability.Tracing;

[Injection(typeof(OpenTelemetryTracingMethodAspect))]
public class OpenTelemetryTracingAttribute : Attribute
{
}
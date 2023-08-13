using AspectInjector.Broker;

namespace dotnet.WebApi.Observability.Tracing;

[Injection(typeof(TracingMethodAspect))]
public class TracingMethodAttribute : Attribute
{
}
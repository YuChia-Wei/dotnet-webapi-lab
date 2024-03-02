using AspectInjector.Broker;

namespace dotnetLab.Observability.Tracing;

[Injection(typeof(TracingMethodAspect))]
public class TracingMethodAttribute : Attribute
{
}
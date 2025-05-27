using AspectInjector.Broker;

namespace dotnetLab.CrossCutting.Observability.Tracing;

[Injection(typeof(TracingMethodAspect))]
public class TracingMethodAttribute : Attribute
{
}
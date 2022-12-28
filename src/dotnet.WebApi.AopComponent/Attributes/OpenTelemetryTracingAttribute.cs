using AspectInjector.Broker;
using dotnet.WebApi.AopComponent.Aspect;

namespace dotnet.WebApi.AopComponent.Attributes;

[Injection(typeof(OpenTelemetryTracingMethodAspect))]
public class OpenTelemetryTracingAttribute : Attribute
{
}
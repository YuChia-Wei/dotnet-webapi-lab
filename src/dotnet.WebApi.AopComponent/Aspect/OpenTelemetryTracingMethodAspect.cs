﻿using AspectInjector.Broker;
using dotnet.WebApi.AopComponent.ActivitySource;

namespace dotnet.WebApi.AopComponent.Aspect;

[AspectInjector.Broker.Aspect(Scope.Global)]
public class OpenTelemetryTracingMethodAspect
{
    [Advice(Kind.Around, Targets = Target.Method | Target.AnyAccess)]
    public object Around(
        [Argument(Source.Name)] string name,
        [Argument(Source.Arguments)] object[] args,
        [Argument(Source.Type)] Type hostType,
        [Argument(Source.Target)] Func<object[], object> target)
    {
        using var startActivity = OpenTelemetryActivitySource.ActiveSource.StartActivity($"{hostType.Name}.{name}");
        var result = target(args);
        return result;
    }
}
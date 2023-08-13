using AspectInjector.Broker;

namespace dotnet.WebApi.Observability.Tracing;

[AspectInjector.Broker.Aspect(Scope.Global)]
public class TracingMethodAspect
{
    [Advice(Kind.Around, Targets = Target.Method | Target.AnyAccess)]
    public object Around(
        [Argument(Source.Name)] string name,
        [Argument(Source.Arguments)] object[] args,
        [Argument(Source.Type)] Type hostType,
        [Argument(Source.Target)] Func<object[], object> target)
    {
        using var startActivity = ObservabilityActivitySource.RegisteredActivity.StartActivity($"{hostType.Name}.{name}");
        var result = target(args);
        return result;
    }
}
using ArchPrecept.Contracts.Application;
using ArchPrecept.Contracts.Domain;
using Wolverine;

namespace dotnetLab.Infrastructure.Messaging;

/// <summary>將程序內領域事件交給 Wolverine；此 adapter 不提供 Outbox 或原子提交保證。</summary>
internal sealed class WolverineDomainEventDispatcher(IMessageBus messageBus) : IDomainEventDispatcher
{
    /// <inheritdoc />
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (var domainEvent in events)
        {
            ct.ThrowIfCancellationRequested();
            await messageBus.PublishAsync(domainEvent);
        }
    }
}
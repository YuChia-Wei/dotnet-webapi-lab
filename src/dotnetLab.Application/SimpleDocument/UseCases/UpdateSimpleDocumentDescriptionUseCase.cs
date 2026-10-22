using ArchPrecept.Contracts.Application;
using dotnetLab.Domain.SampleDoc;

namespace dotnetLab.Application.SimpleDocument.UseCases;

/// <summary>保存文件敘述，成功後才分派領域事件。</summary>
public sealed class UpdateSimpleDocumentDescriptionUseCase(
    IAggregateRepository<SimpleDocumentEntity, int> repository,
#pragma warning disable DBA1016 // 0.0.2-preview misclassifies its own in-process domain-event port by the Dispatcher suffix. See ArchPrecept #7.
    IDomainEventDispatcher eventDispatcher) : IUpdateSimpleDocumentDescriptionUseCase
#pragma warning restore DBA1016
{
    /// <inheritdoc />
    public async Task<bool> ExecuteAsync(UpdateSimpleDocumentDescriptionInput input, CancellationToken cancellationToken)
    {
        var document = await repository.FindByIdAsync(input.SerialId, cancellationToken);
        if (document is null) return false;
        document.UpdateDescription(input.Description);
        await repository.SaveAsync(document, cancellationToken);
        await eventDispatcher.DispatchAsync(document.DomainEvents, cancellationToken);
        document.ClearDomainEvents();
        return true;
    }
}
using ArchPrecept.Contracts.Application;
using dotnetLab.Domain.SampleDoc;

namespace dotnetLab.Application.SimpleDocument.UseCases;

/// <summary>建立文件並以套件聚合儲存契約保存。</summary>
public sealed class CreateSimpleDocumentUseCase(IAggregateRepository<SimpleDocumentEntity, int> repository) : ICreateSimpleDocumentUseCase
{
    /// <inheritdoc />
    public async Task<int> ExecuteAsync(CreateSimpleDocumentInput input, CancellationToken cancellationToken)
    {
        var document = new SimpleDocumentEntity(input.DocumentNum, input.Description);
        await repository.SaveAsync(document, cancellationToken);
        return document.SerialId;
    }
}
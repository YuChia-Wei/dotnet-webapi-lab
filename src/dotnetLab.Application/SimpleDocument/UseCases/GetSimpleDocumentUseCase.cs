using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.Ports.Out;

namespace dotnetLab.Application.SimpleDocument.UseCases;

/// <summary>協調文件唯讀查詢。</summary>
public sealed class GetSimpleDocumentUseCase(ISimpleDocumentQueryRepository repository) : IGetSimpleDocumentUseCase
{
    /// <inheritdoc />
    public Task<SimpleDocumentDto?> ExecuteAsync(GetSimpleDocumentInput input, CancellationToken cancellationToken) =>
        repository.FindByIdAsync(input.SerialId, cancellationToken);
}
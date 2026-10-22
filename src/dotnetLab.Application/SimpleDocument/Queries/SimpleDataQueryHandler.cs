using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.UseCases;

namespace dotnetLab.Application.SimpleDocument.Queries;

/// <summary>將 Wolverine 訊息轉接至單一文件 Use Case。</summary>
public sealed class SimpleDataQueryHandler(IGetSimpleDocumentUseCase useCase)
{
    /// <summary>分派訊息並傳遞取消權杖。</summary>
    public Task<SimpleDocumentDto?> HandleAsync(SimpleDocQuery request, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(new GetSimpleDocumentInput(request.SerialId), cancellationToken);
}
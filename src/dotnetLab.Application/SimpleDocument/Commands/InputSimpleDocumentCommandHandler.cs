using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.UseCases;

namespace dotnetLab.Application.SimpleDocument.Commands;

/// <summary>將 Wolverine 訊息轉接至單一文件 Use Case。</summary>
public sealed class InputSimpleDocumentCommandHandler(ICreateSimpleDocumentUseCase useCase)
{
    /// <summary>分派訊息並傳遞取消權杖。</summary>
    public Task<int> HandleAsync(InputSimpleDocumentCommand request, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(new CreateSimpleDocumentInput(request.DocumentNum, request.Description), cancellationToken);
}
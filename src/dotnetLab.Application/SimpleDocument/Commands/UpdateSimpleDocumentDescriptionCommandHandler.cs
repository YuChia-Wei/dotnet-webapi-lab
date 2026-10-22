using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.UseCases;

namespace dotnetLab.Application.SimpleDocument.Commands;

/// <summary>將 Wolverine 訊息轉接至單一文件 Use Case。</summary>
public sealed class UpdateSimpleDocumentDescriptionCommandHandler(IUpdateSimpleDocumentDescriptionUseCase useCase)
{
    /// <summary>分派訊息並傳遞取消權杖。</summary>
    public Task<bool> HandleAsync(UpdateSimpleDocumentDescriptionCommand request, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(new UpdateSimpleDocumentDescriptionInput(request.SerialId, request.Description), cancellationToken);
}
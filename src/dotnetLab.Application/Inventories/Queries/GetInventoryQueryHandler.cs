using dotnetLab.Application.Inventories.Dtos;
using dotnetLab.Application.Inventories.UseCases;

namespace dotnetLab.Application.Inventories.Queries;

/// <summary>將 Wolverine 查詢訊息轉接至應用程式 Use Case。</summary>
public sealed class GetInventoryQueryHandler(IGetInventoryUseCase useCase)
{
    /// <summary>分派查詢並傳遞取消權杖。</summary>
    public Task<InventoryDto?> HandleAsync(GetInventoryQuery request, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(new GetInventoryInput(request.InventoryId), cancellationToken);
}
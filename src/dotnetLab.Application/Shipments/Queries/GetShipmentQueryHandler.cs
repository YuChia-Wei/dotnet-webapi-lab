using dotnetLab.Application.Shipments.Dtos;
using dotnetLab.Application.Shipments.UseCases;

namespace dotnetLab.Application.Shipments.Queries;

/// <summary>將 Wolverine 查詢訊息轉接至應用程式 Use Case。</summary>
public sealed class GetShipmentQueryHandler(IGetShipmentUseCase useCase)
{
    /// <summary>分派查詢並傳遞取消權杖。</summary>
    public Task<ShipmentDto?> HandleAsync(GetShipmentQuery request, CancellationToken cancellationToken) =>
        useCase.ExecuteAsync(new GetShipmentInput(request.ShipmentId), cancellationToken);
}
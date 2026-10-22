using dotnetLab.Application.Shipments.Dtos;

namespace dotnetLab.Application.Shipments.UseCases;

/// <summary>取得Shipment的業務輸入。</summary>
public sealed record GetShipmentInput(Guid ShipmentId);

/// <summary>提供Shipment查詢的應用程式邊界。</summary>
public interface IGetShipmentUseCase
{
    /// <summary>查詢資料；找不到時回傳 null。</summary>
    Task<ShipmentDto?> ExecuteAsync(GetShipmentInput input, CancellationToken cancellationToken);
}
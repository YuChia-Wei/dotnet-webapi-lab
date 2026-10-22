using ArchPrecept.Contracts.Application;
using dotnetLab.Application.Shipments.Dtos;
using dotnetLab.Domain.Shipments;

namespace dotnetLab.Application.Shipments.UseCases;

/// <summary>
/// 取得貨運處理程序
/// </summary>
public sealed class GetShipmentUseCase : IGetShipmentUseCase
{
    private readonly IAggregateRepository<Shipment, Guid> _shipmentRepository;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="shipmentRepository"></param>
    public GetShipmentUseCase(IAggregateRepository<Shipment, Guid> shipmentRepository)
    {
        this._shipmentRepository = shipmentRepository;
    }

    /// <summary>
    /// 取得貨運
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ShipmentDto?> ExecuteAsync(GetShipmentInput input, CancellationToken cancellationToken)
    {
        var shipment = await this._shipmentRepository.FindByIdAsync(input.ShipmentId, cancellationToken);
        if (shipment == null)
        {
            return null;
        }

        var dto = new ShipmentDto
        {
            Id = shipment.Id,
            TrackingNumber = shipment.TrackingNumber,
            Status = shipment.Status.ToString()
        };

        return dto;
    }
}
using dotnetLab.Application.Shipments.Dtos;
using dotnetLab.Application.Shipments.Ports.Out;

namespace dotnetLab.Application.Shipments.Queries;

/// <summary>
/// 取得貨運處理程序
/// </summary>
public class GetShipmentQueryHandler
{
    private readonly IShipmentRepository _shipmentRepository;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="shipmentRepository"></param>
    public GetShipmentQueryHandler(IShipmentRepository shipmentRepository)
    {
        this._shipmentRepository = shipmentRepository;
    }

    /// <summary>
    /// 取得貨運
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<ShipmentDto?> Handle(GetShipmentQuery request, CancellationToken cancellationToken)
    {
        var shipment = await this._shipmentRepository.GetAsync(request.ShipmentId);
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
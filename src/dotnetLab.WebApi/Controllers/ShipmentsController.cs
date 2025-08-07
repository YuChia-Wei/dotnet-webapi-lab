using dotnetLab.Application.Shipments.Dtos;
using dotnetLab.Application.Shipments.Queries;
using dotnetLab.WebApi.Controllers.ViewModels;
using dotnetLab.WebApi.Infrastructure.ResponseWrapper;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 貨運相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/shipments")]
public class ShipmentsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    /// <summary>
    /// ctor
    /// </summary>
    public ShipmentsController(IMessageBus messageBus)
    {
        this._messageBus = messageBus;
    }

    /// <summary>
    /// 取得貨運
    /// </summary>
    /// <param name="shipmentId"></param>
    /// <returns></returns>
    [HttpGet("{shipmentId}")]
    [ProducesResponseType<ApiResponse<ShipmentViewModel>>(200)]
    public async Task<IActionResult> Get(Guid shipmentId)
    {
        var dto = await this._messageBus.InvokeAsync<ShipmentDto?>(new GetShipmentQuery
        {
            ShipmentId = shipmentId
        });
        if (dto == null)
        {
            return this.NotFound();
        }

        var vm = new ShipmentViewModel
        {
            Id = dto.Id,
            TrackingNumber = dto.TrackingNumber,
            Status = dto.Status
        };
        return this.Ok(vm);
    }
}
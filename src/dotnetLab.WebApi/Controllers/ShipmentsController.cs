using dotnetLab.Application.Shipments.Dtos;
using dotnetLab.Application.Shipments.UseCases;
using dotnetLab.WebApi.ApiInfra.ResponseWrapper;
using dotnetLab.WebApi.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 貨運相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/shipments")]
public class ShipmentsController : ControllerBase
{
    private readonly IGetShipmentUseCase _useCase;

    /// <summary>
    /// ctor
    /// </summary>
    public ShipmentsController(IGetShipmentUseCase useCase)
    {
        this._useCase = useCase;
    }

    /// <summary>
    /// 取得貨運
    /// </summary>
    /// <param name="shipmentId"></param>
    /// <param name="cancellationToken">取消 HTTP 要求的權杖。</param>
    /// <returns></returns>
    [HttpGet("{shipmentId}")]
    [ProducesResponseType<ApiResponse<ShipmentViewModel>>(200)]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(404)]
    public async Task<IActionResult> GetAsync(Guid shipmentId, CancellationToken cancellationToken)
    {
        var dto = await this._useCase.ExecuteAsync(new GetShipmentInput(shipmentId), cancellationToken);
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
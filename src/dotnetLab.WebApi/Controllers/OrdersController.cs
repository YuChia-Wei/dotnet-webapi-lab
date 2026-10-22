using dotnetLab.Application.Orders.Dtos;
using dotnetLab.Application.Orders.UseCases;
using dotnetLab.WebApi.ApiInfra.ResponseWrapper;
using dotnetLab.WebApi.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 訂單相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : ControllerBase
{
    private readonly IGetOrderUseCase _useCase;

    /// <summary>
    /// ctor
    /// </summary>
    public OrdersController(IGetOrderUseCase useCase)
    {
        this._useCase = useCase;
    }

    /// <summary>
    /// 取得訂單
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="cancellationToken">取消 HTTP 要求的權杖。</param>
    /// <returns></returns>
    [HttpGet("{orderId}")]
    [ProducesResponseType<ApiResponse<OrderViewModel>>(200)]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(404)]
    public async Task<IActionResult> GetAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var dto = await this._useCase.ExecuteAsync(new GetOrderInput(orderId), cancellationToken);
        if (dto == null)
        {
            return this.NotFound();
        }

        var vm = new OrderViewModel
        {
            Id = dto.Id,
            CustomerName = dto.CustomerName,
            OrderDate = dto.OrderDate,
            TotalAmount = dto.TotalAmount
        };
        return this.Ok(vm);
    }
}
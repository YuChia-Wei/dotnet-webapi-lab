using System;
using System.Threading.Tasks;
using dotnetLab.Application.Orders.Dtos;
using dotnetLab.Application.Orders.Queries;
using dotnetLab.WebApi.Controllers.ViewModels;
using dotnetLab.WebApi.Infrastructure.ResponseWrapper;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 訂單相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    /// <summary>
    /// ctor
    /// </summary>
    public OrdersController(IMessageBus messageBus)
    {
        this._messageBus = messageBus;
    }

    /// <summary>
    /// 取得訂單
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("{orderId}")]
    [ProducesResponseType<ApiResponse<OrderViewModel>>(200)]
    public async Task<IActionResult> Get(Guid orderId)
    {
        var dto = await this._messageBus.InvokeAsync<OrderDto?>(new GetOrderQuery
        {
            OrderId = orderId
        });
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
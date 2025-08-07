using System;
using System.Threading.Tasks;
using dotnetLab.Application.Inventories.Dtos;
using dotnetLab.Application.Inventories.Queries;
using dotnetLab.WebApi.Controllers.ViewModels;
using dotnetLab.WebApi.Infrastructure.ResponseWrapper;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 庫存相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/inventories")]
public class InventoriesController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    /// <summary>
    /// ctor
    /// </summary>
    public InventoriesController(IMessageBus messageBus)
    {
        this._messageBus = messageBus;
    }

    /// <summary>
    /// 取得庫存
    /// </summary>
    [HttpGet("{inventoryId}")]
    [ProducesResponseType<ApiResponse<InventoryViewModel>>(200)]
    public async Task<IActionResult> Get(Guid inventoryId)
    {
        var dto = await this._messageBus.InvokeAsync<InventoryDto?>(new GetInventoryQuery
        {
            InventoryId = inventoryId
        });
        if (dto == null)
        {
            return this.NotFound();
        }

        var vm = new InventoryViewModel
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity
        };
        return this.Ok(vm);
    }
}
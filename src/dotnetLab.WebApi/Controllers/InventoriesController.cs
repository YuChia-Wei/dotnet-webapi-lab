using dotnetLab.Application.Inventories.Dtos;
using dotnetLab.Application.Inventories.UseCases;
using dotnetLab.WebApi.ApiInfra.ResponseWrapper;
using dotnetLab.WebApi.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 庫存相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/inventories")]
public class InventoriesController : ControllerBase
{
    private readonly IGetInventoryUseCase _useCase;

    /// <summary>
    /// ctor
    /// </summary>
    public InventoriesController(IGetInventoryUseCase useCase)
    {
        this._useCase = useCase;
    }

    /// <summary>
    /// 取得庫存
    /// </summary>
    [HttpGet("{inventoryId}")]
    [ProducesResponseType<ApiResponse<InventoryViewModel>>(200)]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(404)]
    public async Task<IActionResult> GetAsync(Guid inventoryId, CancellationToken cancellationToken)
    {
        var dto = await this._useCase.ExecuteAsync(new GetInventoryInput(inventoryId), cancellationToken);
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
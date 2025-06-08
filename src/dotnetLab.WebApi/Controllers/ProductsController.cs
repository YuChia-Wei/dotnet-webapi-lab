using dotnetLab.UseCases.Products.Dtos;
using dotnetLab.UseCases.Products.Queries;
using dotnetLab.WebApi.Controllers.ViewModels;
using dotnetLab.WebApi.Infrastructure.ResponseWrapper;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 商品相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    /// <summary>
    /// ctor
    /// </summary>
    public ProductsController(IMessageBus messageBus)
    {
        this._messageBus = messageBus;
    }

    /// <summary>
    /// 取得商品
    /// </summary>
    [HttpGet("{productId}")]
    [ProducesResponseType<ApiResponse<ProductViewModel>>(200)]
    public async Task<IActionResult> Get(Guid productId)
    {
        var dto = await this._messageBus.InvokeAsync<ProductDto?>(new GetProductQuery { ProductId = productId });
        if (dto == null)
        {
            return this.NotFound();
        }

        var vm = new ProductViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            UnitPrice = dto.UnitPrice,
            Weight = dto.Weight,
            Dimensions = dto.Dimensions,
            RequiresRefrigeration = dto.RequiresRefrigeration
        };
        return this.Ok(vm);
    }
}

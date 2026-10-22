using dotnetLab.Application.Products.Dtos;
using dotnetLab.Application.Products.UseCases;
using dotnetLab.WebApi.ApiInfra.ResponseWrapper;
using dotnetLab.WebApi.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 商品相關 API
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly IGetProductUseCase _useCase;

    /// <summary>
    /// ctor
    /// </summary>
    public ProductsController(IGetProductUseCase useCase)
    {
        this._useCase = useCase;
    }

    /// <summary>
    /// 取得商品
    /// </summary>
    [HttpGet("{productId}")]
    [ProducesResponseType<ApiResponse<ProductViewModel>>(200)]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(404)]
    public async Task<IActionResult> GetAsync(Guid productId, CancellationToken cancellationToken)
    {
        var dto = await this._useCase.ExecuteAsync(new GetProductInput(productId), cancellationToken);
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
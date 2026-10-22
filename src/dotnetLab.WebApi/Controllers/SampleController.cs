using dotnetLab.Application.SimpleDocument.UseCases;
using dotnetLab.WebApi.ApiInfra.ParameterValidators;
using dotnetLab.WebApi.ApiInfra.ResponseWrapper;
using dotnetLab.WebApi.Controllers.Requests;
using dotnetLab.WebApi.Controllers.Validators;
using dotnetLab.WebApi.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>文件查詢、寫入與回應包裝範例。</summary>
[ApiController]
[Route("api/v{version:apiVersion}/Sample")]
public sealed class SampleController(
    IGetSimpleDocumentUseCase getDocument,
    ICreateSimpleDocumentUseCase createDocument,
    IUpdateSimpleDocumentDescriptionUseCase updateDocument) : ControllerBase
{
    /// <summary>查詢文件；找不到時回傳 404。</summary>
    [HttpGet]
    [HttpGet("use-fluent-validation")]
    [ParameterValidator<SimpleDocQueryValidator>]
    [ProducesResponseType<ApiResponse<SimpleDocumentViewModel>>(200)]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(404)]
    public async Task<IActionResult> GetAsync([FromQuery] SimpleDocQueryRequest request, CancellationToken cancellationToken)
    {
        var dto = await getDocument.ExecuteAsync(new GetSimpleDocumentInput(request.SerialId), cancellationToken);
        return dto is null
            ? this.NotFound()
            : this.Ok(new SimpleDocumentViewModel { SerialId = dto.SerialId, Description = dto.Description, DocumentNum = dto.DocumentNum });
    }

    /// <summary>示範 DataAnnotations 驗證。</summary>
    [HttpGet("use-data-annotations-validation")]
    [ProducesResponseType<ApiResponse<string>>(200)]
    public IActionResult GetDataAnnotations([FromQuery] DataAnnotationsValidateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return this.Ok(string.Empty);
    }

    /// <summary>示範 IActionResult 包裝。</summary>
    [HttpGet("action-result")]
    [ProducesResponseType<ApiResponse<string>>(200)]
    public IActionResult GetActionResult() => this.Ok(string.Empty);

    /// <summary>示範一般例外回應。</summary>
    [HttpGet("exception/dotnet-exception")]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(500)]
    public IActionResult GetDotnetExceptionResponse() => throw new ArgumentOutOfRangeException();

    /// <summary>示範業務錯誤回應。</summary>
    [HttpGet("exception/custom-exception")]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(500)]
    public IActionResult GetExceptionResponse() => throw new ErrorCodeException("err-my-error");

    /// <summary>示範一般物件包裝。</summary>
    [HttpGet("object")]
    [ProducesResponseType<ApiResponse<string>>(200)]
    public IActionResult GetObject() => this.Ok("Hello World");

    /// <summary>建立文件並回傳序號。</summary>
    [HttpPost]
    [ParameterValidator<NewSimpleDocumentRequestValidator>]
    [ProducesResponseType<ApiResponse<int>>(200)]
    public async Task<IActionResult> PostAsync([FromBody] NewSimpleDocumentRequest request, CancellationToken cancellationToken)
    {
        var id = await createDocument.ExecuteAsync(new CreateSimpleDocumentInput(request.DocumentNum, request.Description), cancellationToken);
        return this.Ok(id);
    }

    /// <summary>更新文件敘述並回傳布林結果。</summary>
    [HttpPatch]
    [ParameterValidator<UpdateSimpleDocumentDescriptionRequestValidator>]
    [ProducesResponseType<ApiResponse<bool>>(200)]
    public async Task<IActionResult> PatchAsync([FromBody] UpdateSimpleDocumentDescriptionRequest request, CancellationToken cancellationToken)
    {
        var updated = await updateDocument.ExecuteAsync(new UpdateSimpleDocumentDescriptionInput(request.SerialId, request.Description), cancellationToken);
        return this.Ok(updated);
    }
}
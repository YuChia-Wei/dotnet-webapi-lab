using dotnetLab.UseCase.SimpleDocument.Commands;
using dotnetLab.UseCase.SimpleDocument.Queries;
using dotnetLab.WebApi.Controllers.Requests;
using dotnetLab.WebApi.Controllers.Validator;
using dotnetLab.WebApi.Controllers.ViewModels;
using dotnetLab.WebApi.Infrastructure.ParameterValidators;
using dotnetLab.WebApi.Infrastructure.ResponseWrapper;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// Sample Api (use CQRS Pattern)
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/Sample")]
public class SampleController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="mediator"></param>
    public SampleController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// get simple doc
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [HttpGet("use-fluent-validation")]
    [ParameterValidator<SimpleDocQueryValidator>]
    // [ApiPermissionAuthorize(Policy = "SamplePolicy",
    //                         Roles = "Admin",
    //                         AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
    //                         PermissionRule = "sampleAPIPermissionRule")]
    // [ApiPermissionAuthorize("SamplePolicy", "sampleAPIPermissionRule")]
    // [ApiPermissionAuthorize("sampleAPIPermissionRule")]
    [ProducesResponseType<ApiResponse<SimpleDocumentViewModel>>(200)]
    public async Task<IActionResult> Get([FromQuery] SimpleDocQueryRequest request)
    {
        var query = new SimpleDocQuery()
        {
            SerialId = request.SerialId
        };
        var simpleDocumentDto = await this._mediator.Send(query);
        var simpleDocumentViewModel = new SimpleDocumentViewModel
        {
            SerialId = simpleDocumentDto.SerialId,
            Description = simpleDocumentDto.Description,
            DocumentNum = simpleDocumentDto.DocumentNum
        };
        return this.Ok(simpleDocumentViewModel);
    }

    /// <summary>
    /// test api response warpper
    /// </summary>
    /// <returns></returns>
    [HttpGet("use-data-annotations-validation")]
    [ProducesResponseType<ApiResponse<SimpleDocumentViewModel>>(200)]
    public async Task<IActionResult> Get([FromQuery] DataAnnotationsValidateRequest request)
    {
        return this.Ok("");
    }

    /// <summary>
    /// test api response warpper
    /// </summary>
    /// <returns></returns>
    [HttpGet("action-result")]
    public async Task<IActionResult> GetActionResult()
    {
        return this.Ok("");
    }

    /// <summary>
    /// get sample exception response
    /// </summary>
    /// <returns></returns>
    [HttpGet("error")]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(200)]
    public async Task<IActionResult> GetExceptionResponse()
    {
        throw new AggregateException();
    }

    /// <summary>
    /// test api response warpper
    /// </summary>
    /// <returns></returns>
    [HttpGet("object")]
    public async Task<object> GetObject()
    {
        return "Hello World";
    }

    /// <summary>
    /// create simple doc
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ParameterValidator<NewSimpleDocumentRequestValidator>]
    [ProducesResponseType<ApiResponse<int>>(200)]
    public async Task<IActionResult> Post([FromBody] NewSimpleDocumentRequest request)
    {
        var inputSimpleDocumentCommand = new InputSimpleDocumentCommand
        {
            DocumentNum = request.DocumentNum,
            Description = request.Description
        };
        var send = await this._mediator.Send(inputSimpleDocumentCommand);
        return this.Ok(send);
    }

    /// <summary>
    /// update simple doc Description
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch]
    [ParameterValidator<UpdateSimpleDocumentDescriptionRequestValidator>]
    [ProducesResponseType<ApiResponse<bool>>(200)]
    public async Task<IActionResult> Post([FromBody] UpdateSimpleDocumentDescriptionRequest request)
    {
        var command = new UpdateSimpleDocumentDescriptionCommand
        {
            SerialId = request.SerialId,
            Description = request.Description
        };
        var send = await this._mediator.Send(command);
        return this.Ok(send);
    }
}
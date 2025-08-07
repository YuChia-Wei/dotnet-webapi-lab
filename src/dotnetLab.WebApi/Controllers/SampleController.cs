using System;
using System.Threading.Tasks;
using dotnetLab.Application.SimpleDocument.Commands;
using dotnetLab.Application.SimpleDocument.Dtos;
using dotnetLab.Application.SimpleDocument.Queries;
using dotnetLab.WebApi.Controllers.Requests;
using dotnetLab.WebApi.Controllers.Validator;
using dotnetLab.WebApi.Controllers.ViewModels;
using dotnetLab.WebApi.Infrastructure.ParameterValidators;
using dotnetLab.WebApi.Infrastructure.ResponseWrapper;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// Sample Api (use CQRS Pattern)
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/Sample")]
public class SampleController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="messageBus"></param>
    public SampleController(IMessageBus messageBus)
    {
        this._messageBus = messageBus;
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
        var query = new SimpleDocQuery
        {
            SerialId = request.SerialId
        };
        var simpleDocumentDto = await this._messageBus.InvokeAsync<SimpleDocumentDto>(query);
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
    /// get error response by dotnet exception
    /// </summary>
    /// <returns></returns>
    [HttpGet("exception/dotnet-exception")]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(200)]
    public async Task<IActionResult> GetDotnetExceptionResponse()
    {
        throw new ArgumentOutOfRangeException();
    }

    /// <summary>
    /// get error response by custom exception
    /// </summary>
    /// <returns></returns>
    [HttpGet("exception/custom-exception")]
    [ProducesResponseType<ApiResponse<ApiErrorInformation>>(200)]
    public async Task<IActionResult> GetExceptionResponse()
    {
        throw new ErrorCodeException("err-my-error");
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
        var send = await this._messageBus.InvokeAsync<int>(inputSimpleDocumentCommand);
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
        var send = await this._messageBus.InvokeAsync<int>(command);
        return this.Ok(send);
    }
}
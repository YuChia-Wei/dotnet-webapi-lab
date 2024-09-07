using dotnetLab.UseCase.SimpleDocument.Commands;
using dotnetLab.UseCase.SimpleDocument.Queries;
using dotnetLab.WebApi.Controllers.Validator;
using dotnetLab.WebApi.Infrastructure.Attributes;
using dotnetLab.WebApi.Infrastructure.Authorization.Policy;
using Mediator;
using Microsoft.AspNetCore.Authorization;
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
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpGet]
    [ParameterValidator<SimpleDocQueryValidator>]
    [Authorize]
    public async Task<IActionResult> Get([FromQuery] SimpleDocQuery command)
    {
        return this.Ok(await this._mediator.Send(command));
    }

    /// <summary>
    /// create simple doc
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    [ParameterValidator<InputSimpleDocumentCommandValidator>]
    // [Authorize(nameof(LoginUserRequestedPolicy))]
    public async Task<IActionResult> Post([FromBody] InputSimpleDocumentCommand command)
    {
        var send = await this._mediator.Send(command);
        return this.Ok(send);
    }

    /// <summary>
    /// update simple doc Description
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPatch]
    [ParameterValidator<UpdateSimpleDocumentDescriptionCommandValidator>]
    // [Authorize(nameof(LoginUserRequestedPolicy))]
    public async Task<IActionResult> Post([FromBody] UpdateSimpleDocumentDescriptionCommand command)
    {
        var send = await this._mediator.Send(command);
        return this.Ok(send);
    }
}
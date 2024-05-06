using dotnetLab.UseCase.SimpleDocument.Commands;
using dotnetLab.UseCase.SimpleDocument.Queries;
using dotnetLab.WebApi.Controllers.Validator;
using dotnetLab.WebApi.Infrastructure.Attributes;
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
    /// 僅需要 Client Credentials 認證
    /// </summary>
    /// <param name="dataCommand"></param>
    /// <returns></returns>
    [HttpGet]
    [ParameterValidator<SimpleDocQueryValidator>]
    // [Authorize]
    public async Task<IActionResult> Get([FromQuery] SimpleDocQuery dataCommand)
    {
        return this.Ok(await this._mediator.Send(dataCommand));
    }

    /// <summary>
    /// 需要 LoginUser 的認證 (使用有身分的 OAuth 流程)
    /// </summary>
    /// <param name="dataCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [ParameterValidator<InputSimpleDocumentCommandValidator>]
    // [Authorize(nameof(LoginUserRequestedPolicy))]
    public async Task<IActionResult> Post([FromBody] InputSimpleDocumentCommand dataCommand)
    {
        var send = await this._mediator.Send(dataCommand);
        return this.Ok(send);
    }
}
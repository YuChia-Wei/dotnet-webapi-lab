using dotnet.WebApi.Infrastructure.Authorization.Policy;
using dotnet.WebApi.Service.Commands;
using dotnet.WebApi.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.WebApi.ApiControllers;

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
    [Authorize]
    public async Task<IActionResult> Get([FromQuery] SampleDataQuery dataCommand)
    {
        return this.Ok(await this._mediator.Send(dataCommand));
    }

    /// <summary>
    /// 需要 LoginUser 的認證 (使用有身分的 OAuth 流程)
    /// </summary>
    /// <param name="dataCommand"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(nameof(LoginUserRequestedPolicy))]
    public async Task<IActionResult> Post([FromBody] InputSampleDataCommand dataCommand)
    {
        return this.Ok(await this._mediator.Send(dataCommand));
    }
}
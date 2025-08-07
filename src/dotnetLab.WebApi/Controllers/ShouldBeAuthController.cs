using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// 表示需要授權的 API 控制器。
/// </summary>
[Authorize]
[ApiController]
[Route("api/v{version:apiVersion}/ShouldBeAuth")]
public class ShouldBeAuthController : ControllerBase
{
    /// <summary>
    /// get ok
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Task<IActionResult> Get()
    {
        return Task.FromResult<IActionResult>(this.Ok());
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

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
    public async Task<IActionResult> Get()
    {
        return this.Ok();
    }
}
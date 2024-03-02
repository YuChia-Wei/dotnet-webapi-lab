using Microsoft.AspNetCore.Mvc;

namespace dotnetLab.WebApi.Controllers;

/// <summary>
/// index
/// </summary>
public class HomeController : Controller
{
    // GET
    /// <summary>
    /// Index Page (default redirect to swagger)
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return this.Redirect("swagger");
    }
}
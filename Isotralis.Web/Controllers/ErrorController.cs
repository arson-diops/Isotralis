using Microsoft.AspNetCore.Mvc;

namespace Isotralis.Web.Controllers;

[Route("[controller]/[action]")]
[IgnoreAntiforgeryToken]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class ErrorController : Controller
{
    [HttpGet("{statusCode:int}")]
    public IActionResult Index(int statusCode) => View(statusCode);
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Isotralis.Web.Areas.Review.Controllers;

[Area("Review")]
[Route("[area]/[controller]/[action]")]
[Authorize(Roles = Constants.TechnicianUserRole + "," + Constants.SupervisorUserRole)]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

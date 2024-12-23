using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Isotralis.Web.Areas.Sample.Controllers;

[Area("Sample")]
[Route("[area]/[controller]/[action]")]
[Authorize(Roles = Constants.GeneralUserRole + "," + Constants.TechnicianUserRole + "," + Constants.SupervisorUserRole)]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

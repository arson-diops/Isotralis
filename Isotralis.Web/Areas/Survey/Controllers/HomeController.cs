using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Isotralis.Web.Areas.Survey.Controllers;

[Area("Survey")]
[Route("[area]/[controller]/[action]")]
[Authorize(Roles = Constants.GeneralUserRole + "," + Constants.TechnicianUserRole + "," + Constants.SupervisorUserRole)]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

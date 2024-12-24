using System.Diagnostics;
using Isotralis.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Isotralis.Web.Controllers
{
    [Authorize(Roles = Constants.GeneralUserRole + "," + Constants.TechnicianUserRole + "," + Constants.SupervisorUserRole)]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogDebug("Entered GET HomeController::Index");

            LandingPageModel model = new()
            {
                Cards =
                [
                    new CardModel("Document a Sample", "Complete radiological samples from the air, ground, or otherwise.", "Index", "Home", "Sample", Constants.GeneralUserRole),
                    new CardModel("Document a Survey", "Create, edit, and manage radiological surveys.", "Index", "Home", "Survey", Constants.GeneralUserRole),
                    new CardModel("Review Radiological Documents", "Review and moderate radiological surveys and samples.", "Index", "Home", "Review", Constants.SupervisorUserRole)
                ]
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
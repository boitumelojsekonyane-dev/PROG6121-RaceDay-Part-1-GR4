
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message =
                "Welcome to your RACEDAY Organiser Dashboard!";

            return View();
        }
    }
}

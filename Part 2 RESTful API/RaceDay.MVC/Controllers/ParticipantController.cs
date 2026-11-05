
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    [Authorize(Roles = "Participant")]
    public class ParticipantController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message =
                "Welcome to your RACEDAY Participant Dashboard!";

            return View();
        }
    }
}

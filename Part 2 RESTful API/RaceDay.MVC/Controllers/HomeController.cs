
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using System.Net.Http;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventApiService _eventService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            EventApiService eventService,
            ILogger<HomeController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                List<JsonElement> events =
                    await _eventService.GetEventsAsync();

                ViewBag.Events = events;
                ViewBag.ErrorMessage = null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "Unable to retrieve events from RaceDay.API");

                ViewBag.Events = new List<JsonElement>();
                ViewBag.ErrorMessage =
                    "Events are temporarily unavailable. Please try again later.";
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Invalid event data received from RaceDay.API");

                ViewBag.Events = new List<JsonElement>();
                ViewBag.ErrorMessage =
                    "Event information could not be loaded.";
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                JsonElement? eventDetails =
                    await _eventService.GetEventByIdAsync(id);

                if (!eventDetails.HasValue)
                {
                    return NotFound();
                }

                return View(eventDetails.Value);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "Unable to retrieve event {EventId}", id);

                TempData["ErrorMessage"] =
                    "Event details are temporarily unavailable.";

                return RedirectToAction(nameof(Index));
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Invalid details received for event {EventId}", id);

                TempData["ErrorMessage"] =
                    "The selected event could not be displayed.";

                return RedirectToAction(nameof(Index));
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new RaceDay.MVC.Models.ErrorViewModel
            {
                RequestId =
                    System.Diagnostics.Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        private readonly EventApiService _eventService;

        public OrganiserController(EventApiService eventService)
        {
            _eventService = eventService;
        }

        // Organiser Dashboard

        public async Task<IActionResult> Index()
        {
            ViewBag.Message =
                "Welcome to your RACEDAY Organiser Dashboard!";

            try
            {
                var events = await _eventService.GetEventsAsync();
                ViewBag.Events = events;
            }
            catch (HttpRequestException)
            {
                ViewBag.Events = new List<JsonElement>();

                TempData["ErrorMessage"] =
                    "Unable to load events at the moment.";
            }

            return View();
        }

        // Display the Create Event form.
        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            try
            {
                await LoadEventTypesAsync();
                return View(new CreateEventViewModel());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Event types are temporarily unavailable.";

                return RedirectToAction(nameof(Index));
            }
        }

        // Submit a new event to the API.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(
            CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadEventTypesAsync();
                return View(model);
            }

            var token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login", "Account");
            }

            try
            {
                using var response =
                    await _eventService.CreateEventAsync(
                        model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event created successfully!";

                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content
                    .ReadAsStringAsync();

                ModelState.AddModelError(
                    "",
                    $"Event creation failed: {error}");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "",
                    "The event service is temporarily unavailable.");
            }

            await LoadEventTypesAsync();
            return View(model);
        }


        // GET: Organiser/EditEvent/5
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (id <= 0)
                return BadRequest();

            var token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            try
            {
                var eventData = await _eventService.GetEventByIdAsync(id);

                if (!eventData.HasValue)
                    return NotFound();

                var e = eventData.Value;

                // Check ownership before displaying the edit form.
                var adminIdClaim = User.FindFirst("adminId")?.Value;

                if (!int.TryParse(adminIdClaim, out var adminId) ||
                    e.GetProperty("adminID").GetInt32() != adminId)
                {
                    return Forbid();
                }

                var model = new EditEventViewModel
                {
                    EventID = e.GetProperty("eventID").GetInt32(),
                    EventTypeID = e.GetProperty("eventTypeID").GetInt32(),
                    Title = e.GetProperty("title").GetString() ?? "",
                    Description = e.GetProperty("description").GetString() ?? "",
                    EventDate = e.GetProperty("eventDate").Deserialize<DateOnly>(),
                    StartTime = e.GetProperty("startTime").Deserialize<TimeOnly>(),
                    Location = e.GetProperty("location").GetString() ?? "",
                    DistanceKm = e.GetProperty("distanceKm").GetDecimal(),
                    RegistrationOpens = e.GetProperty("registrationOpens").Deserialize<DateOnly>(),
                    RegistrationCloses = e.GetProperty("registrationCloses").Deserialize<DateOnly>(),
                    EntryFee = e.GetProperty("entryFee").GetDecimal()
                };

                await LoadEventTypesAsync();

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to load event details.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Organiser/EditEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(
            int id, EditEventViewModel model)
        {
            if (id <= 0 || id != model.EventID)
                return BadRequest();

            var token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await LoadEventTypesAsync();
                return View(model);
            }

            try
            {
                using var response =
                    await _eventService.UpdateEventAsync(
                        id, model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event updated successfully!";

                    return RedirectToAction(nameof(Index));
                }

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.Forbidden)
                {
                    return Forbid();
                }

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    "",
                    $"Event update failed: {error}");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "",
                    "The event service is temporarily unavailable.");
            }

            await LoadEventTypesAsync();
            return View(model);
        }


        // GET: Organiser/DeleteEvent/5
        [HttpGet]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var eventData =
                    await _eventService.GetEventByIdAsync(id);

                if (!eventData.HasValue)
                    return NotFound();

                var e = eventData.Value;

                var adminIdClaim =
                    User.FindFirst("adminId")?.Value;

                if (!int.TryParse(adminIdClaim, out int adminId) ||
                    e.GetProperty("adminID").GetInt32() != adminId)
                {
                    return Forbid();
                }

                ViewBag.EventID = id;
                ViewBag.EventTitle =
                    e.GetProperty("title").GetString();

                return View();
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to load event details.";

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Organiser/DeleteEvent/5
        [HttpPost]
        [ActionName("DeleteEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEventConfirmed(int id)
        {
            if (id <= 0)
                return BadRequest();

            var token =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Account");

            try
            {
                using var response =
                    await _eventService.DeleteEventAsync(id, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Event deleted successfully!";

                    return RedirectToAction(nameof(Index));
                }

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.Forbidden)
                {
                    return Forbid();
                }

                if (response.StatusCode ==
                    System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                var error =
                    await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] =
                    $"Event deletion failed: {error}";
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "The event service is temporarily unavailable.";
            }

            return RedirectToAction(nameof(Index));
        }


        // Populate the event type dropdown.
        private async Task LoadEventTypesAsync()
        {
            var eventTypes =
                await _eventService.GetEventTypesAsync();

            ViewBag.EventTypes = eventTypes.Select(e =>
                new SelectListItem
                {
                    Value = e.GetProperty("eventTypeID")
                        .GetInt32().ToString(),

                    Text = e.GetProperty("typeName")
                        .GetString() ?? "Unknown"
                }).ToList();
        }
    }
}

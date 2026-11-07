
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    [Authorize(Roles = "Participant")]
    public class EnrolmentController : Controller
    {
        private readonly EnrolmentApiService _enrolmentService;
        private readonly ILogger<EnrolmentController> _logger;

        public EnrolmentController(
            EnrolmentApiService enrolmentService,
            ILogger<EnrolmentController> logger)
        {
            _enrolmentService = enrolmentService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int eventId,
            int eventCategoryId)
        {
            if (eventId <= 0 || eventCategoryId <= 0)
            {
                return BadRequest();
            }

            string? accessToken =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["ErrorMessage"] =
                    "Your session has expired. Please log in again.";

                return RedirectToAction(
                    "Login", "Account");
            }

            try
            {
                string confirmationCode =
                    await _enrolmentService.CreateEnrolmentAsync(
                        eventCategoryId,
                        accessToken);

                TempData["SuccessMessage"] =
                    $"Enrolment successful! Confirmation code: {confirmationCode}";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "Unable to create enrolment for category {CategoryId}",
                    eventCategoryId);

                TempData["ErrorMessage"] =
                    "The enrolment service is temporarily unavailable.";
            }

            return RedirectToAction(
                "Details",
                "Home",
                new { id = eventId });
        }
    }
}

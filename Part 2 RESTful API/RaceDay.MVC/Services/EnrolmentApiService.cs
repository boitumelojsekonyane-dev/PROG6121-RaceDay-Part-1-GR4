
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Services
{
    public class EnrolmentApiService
    {
        private readonly HttpClient _httpClient;

        public EnrolmentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateEnrolmentAsync(
            int eventCategoryId,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/enrolments");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer", accessToken);

            request.Content = JsonContent.Create(new
            {
                EventCategoryID = eventCategoryId
            });

            using var response =
                await _httpClient.SendAsync(request);

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var result = document.RootElement;

            string message = result.TryGetProperty(
                "message", out var messageElement)
                ? messageElement.GetString() ?? "Unknown response."
                : "The enrolment request could not be completed.";

            if (response.StatusCode == HttpStatusCode.Created)
            {
                string confirmationCode =
                    result.TryGetProperty(
                        "confirmationCode",
                        out var confirmationElement)
                    ? confirmationElement.GetString() ?? ""
                    : "";

                return confirmationCode;
            }

            throw new InvalidOperationException(message);
        }
    }
}


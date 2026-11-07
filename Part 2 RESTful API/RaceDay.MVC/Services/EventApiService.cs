
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Services
{
    public class EventApiService
    {
        private readonly HttpClient _httpClient;

        public EventApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Retrieve all available events.
        public async Task<List<JsonElement>> GetEventsAsync()
        {
            using var response =
                await _httpClient.GetAsync("api/events");

            response.EnsureSuccessStatusCode();

            var events = await response.Content
                .ReadFromJsonAsync<List<JsonElement>>();

            return events ?? new List<JsonElement>();
        }

        // Retrieve the details of a specific event.
        public async Task<JsonElement?> GetEventByIdAsync(
            int eventId)
        {
            using var response =
                await _httpClient.GetAsync(
                    $"api/events/{eventId}");

            if (response.StatusCode ==
                System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var eventDetails =
                await response.Content
                    .ReadFromJsonAsync<JsonElement>();

            return eventDetails;
        }


        public async Task<List<JsonElement>> GetEventCategoriesAsync(
            int eventId)
        {
            using var response = await _httpClient.GetAsync(
                $"api/events/{eventId}/categories");

            response.EnsureSuccessStatusCode();

            var categories = await response.Content
                .ReadFromJsonAsync<List<JsonElement>>();

            return categories ?? new List<JsonElement>();
        }

    }
}

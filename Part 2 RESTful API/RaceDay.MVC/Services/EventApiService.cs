
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

        public async Task<List<JsonElement>> GetEventsAsync()
        {
            using var response =
                await _httpClient.GetAsync("api/events");

            response.EnsureSuccessStatusCode();

            var events = await response.Content
                .ReadFromJsonAsync<List<JsonElement>>();

            return events ?? new List<JsonElement>();
        }
    }
}

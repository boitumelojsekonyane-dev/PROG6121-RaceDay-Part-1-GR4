
using System.Net.Http.Json;
using System.Text.Json;
using System.Net;
using System.Net.Http.Headers;

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


        // Create a new event using the organiser's JWT.
        public async Task<HttpResponseMessage> CreateEventAsync(
            object eventData,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, "api/events");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = JsonContent.Create(eventData);

            return await _httpClient.SendAsync(request);
        }

        // Update an event using the organiser's JWT.
        public async Task<HttpResponseMessage> UpdateEventAsync(
            int eventId,
            object eventData,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put, $"api/events/{eventId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = JsonContent.Create(eventData);

            return await _httpClient.SendAsync(request);
        }

        // Delete an event using the organiser's JWT.
        public async Task<HttpResponseMessage> DeleteEventAsync(
            int eventId,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete, $"api/events/{eventId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            return await _httpClient.SendAsync(request);
        }


        // Retrieve active event types for the organiser form.
        public async Task<List<JsonElement>> GetEventTypesAsync()
        {
            using var response =
                await _httpClient.GetAsync("api/events/types");

            response.EnsureSuccessStatusCode();

            var eventTypes = await response.Content
                .ReadFromJsonAsync<List<JsonElement>>();

            return eventTypes ?? new List<JsonElement>();
        }


        // Retrieve a specific event category.
        public async Task<JsonElement?> GetEventCategoryByIdAsync(
            int eventId,
            int categoryId)
        {
            using var response = await _httpClient.GetAsync(
                $"api/events/{eventId}/categories/{categoryId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<JsonElement>();
        }

        // Create a category using the organiser's JWT.
        public async Task<HttpResponseMessage> CreateEventCategoryAsync(
            int eventId,
            object categoryData,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"api/events/{eventId}/categories");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = JsonContent.Create(categoryData);

            return await _httpClient.SendAsync(request);
        }

        // Update an existing event category.
        public async Task<HttpResponseMessage> UpdateEventCategoryAsync(
            int eventId,
            int categoryId,
            object categoryData,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put,
                $"api/events/{eventId}/categories/{categoryId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = JsonContent.Create(categoryData);

            return await _httpClient.SendAsync(request);
        }

        // Delete an event category.
        public async Task<HttpResponseMessage> DeleteEventCategoryAsync(
            int eventId,
            int categoryId,
            string accessToken)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/events/{eventId}/categories/{categoryId}");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            return await _httpClient.SendAsync(request);
        }

    }
}

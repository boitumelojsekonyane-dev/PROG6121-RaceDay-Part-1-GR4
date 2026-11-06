
using System.Net.Http.Json;
using System.Text.Json;
using RaceDay.MVC.Models;

namespace RaceDay.MVC.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Register a new Participant

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/register", model);

            if (response.StatusCode ==
                System.Net.HttpStatusCode.Conflict)
            {
                using var json =
                    await response.Content.ReadFromJsonAsync<JsonDocument>();

                string message = "An account with these details already exists.";

                if (json != null &&
                    json.RootElement.TryGetProperty(
                        "message", out var messageElement))
                {
                    message = messageElement.GetString() ?? message;
                }

                throw new InvalidOperationException(message);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Registration API returned HTTP {(int)response.StatusCode}.");
            }

            return true;
        }


        // Login as Participant or Organiser

        public async Task<string?> LoginAsync(LoginViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/login",
                new
                {
                    model.Email,
                    model.Password
                });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            using var json =
                await response.Content.ReadFromJsonAsync<JsonDocument>();

            if (json == null ||
                !json.RootElement.TryGetProperty(
                    "token", out var tokenElement))
            {
                return null;
            }

            return tokenElement.GetString();
        }

    }
}

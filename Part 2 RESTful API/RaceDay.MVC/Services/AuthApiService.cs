
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
        public async Task<bool> RegisterAsync(
            RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/register", model);

            return response.IsSuccessStatusCode;
        }

        // Login as Participant or Organiser
        public async Task<string?> LoginAsync(
            LoginViewModel model)
        {
            string endpoint = model.Role switch
            {
                "Participant" => "api/auth/login",
                "Organiser" => "api/auth/organiser/login",
                _ => throw new ArgumentException(
                    "Invalid account type.")
            };

            var response = await _httpClient.PostAsJsonAsync(
                endpoint,
                new
                {
                    model.Email,
                    model.Password
                });

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            using var json = await response.Content
                .ReadFromJsonAsync<JsonDocument>();

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

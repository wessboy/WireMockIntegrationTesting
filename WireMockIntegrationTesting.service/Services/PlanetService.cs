
using System.Net;
using System.Text.Json;
using WireMockIntegrationTesting.service.Models;

namespace WireMockIntegrationTesting.service.Services;
     public class PlanetService
    { 
        private readonly HttpClient _httpClient;
        public PlanetService(HttpClient httpClient) => _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient)); 
    
         public async Task<Planet?> GetPlanetByIdAsync(int id)
         {
            var response = await _httpClient.GetAsync($"planets/{id}");
            if(response.StatusCode == HttpStatusCode.OK)
            {
                 return JsonSerializer.Deserialize<Planet>(await response.Content.ReadAsStringAsync());
            }

             return null;
         }
     }


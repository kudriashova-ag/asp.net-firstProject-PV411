using System.Net;
using MyApp.Features.Weather.DTO;

namespace MyApp.Services;

public class WeatherClient : IWeatherClient
{
    private readonly HttpClient _client;
    private readonly string _apiKey;

    public WeatherClient(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _apiKey = configuration["WeatherApi:ApiKey"]!;
    }

    public async Task<WeatherResponse?> GetByCityAsync(string city)
    {
        try
        {
            var response = await _client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // logger
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<WeatherResponse>();

        }
        catch (HttpRequestException ex)
        {
            // logger
            throw;
        }
    }
}
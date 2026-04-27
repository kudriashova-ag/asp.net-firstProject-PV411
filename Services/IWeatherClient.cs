using MyApp.Features.Weather.DTO;

namespace MyApp.Services;

public interface IWeatherClient
{
    Task<WeatherResponse?> GetByCityAsync(string city);
}
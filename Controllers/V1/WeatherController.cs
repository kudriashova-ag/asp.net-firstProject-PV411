using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyApp.Services;

namespace MyApp.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class WeatherController(IWeatherClient weatherClient) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWeatherToday()
    {
        var response = await weatherClient.GetByCityAsync("Kyiv");
        return Ok(response);
    }



}
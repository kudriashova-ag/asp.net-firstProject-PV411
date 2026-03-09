using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;

namespace MyApp.Controllers.V2;

/// <summary>
/// Операції з фільмами: перегляд, пошук, додавання, редагування та видалення кінематографічних творів
/// </summary>
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public class MovieController : ControllerBase
{
    private static List<Movie> _movies = new()
    {
        new Movie{Id=1, Title="The Shawshank Redemption", Year=1994, Director="Frank Darabont", Genre="Drama"},
        new Movie{Id=2, Title="The Godfather", Year=1972, Director="Francis Ford Coppola", Genre="Drama"},
        new Movie{Id=3, Title="The Dark Knight", Year=2008, Director="Christopher Nolan", Genre="Action"},
    };

    /// <summary>
    /// Повертає всі фільми
    /// </summary>
    /// <returns>Список фільмів</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public ActionResult<IEnumerable<Movie>> Get()
    {
        return Ok(_movies);
    }
}



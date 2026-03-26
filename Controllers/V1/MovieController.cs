using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Movie;
using MyApp.Helpers.Pagination;
using MyApp.Helpers.QueryParameters;
using MyApp.Services;
using MyApp.Validators.Movie;
using Swashbuckle.AspNetCore.Annotations;

namespace MyApp.Controllers.V1;

/// <summary>
/// Операції з фільмами: перегляд, пошук, додавання, редагування та видалення кінематографічних творів
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
[SwaggerTag("Операції з фільмами")]
public class MovieController : ControllerBase
{

    private readonly IMovieService _movieService;
    private readonly CreateMovieRequestValidator _createMovieValidator;

    public MovieController(IMovieService movieService, CreateMovieRequestValidator createMovieValidator)
    {
        _movieService = movieService;
        _createMovieValidator = createMovieValidator;
    }


    /// <summary>
    /// Повертає всі фільми
    /// </summary>
    /// <param name="ct">CancellationToken</param>
    /// <param name="parameters">Параметри пошуку</param>
    /// <returns>Список фільмів</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public async Task<ActionResult<PagedResult<MovieSummaryDto>>> Get(CancellationToken ct, [FromQuery] MovieQueryParameters parameters)
    {
        var movieSummary = await _movieService.GetAllMovies(ct, parameters);
        return Ok(movieSummary);
    }

    /// <summary>
    /// Повертає фільм за Id
    /// </summary>
    /// <param name="id">Ідентифікатор фільму</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Об'єкт Movie або помилка</returns>
    /// <response code="200">Успішне отримання фільму</response>
    /// <response code="400">Некоректний Id</response>
    /// <response code="404">Фільм не знайдено</response>
    /// <response code="500">Помилка сервера</response>
    /// <remarks> Метод шукає фільм за Id, який передається як параметр.</remarks>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MovieDetailDto>> GetById(int id, CancellationToken ct)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "Id must be greater than 0" });
        }

        var movie = await _movieService.GetMovieById(id, ct);

        if (movie == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        return Ok(movie);
    }


    /// <summary>
    /// Створює новий фільм
    /// </summary>
    /// <param name="movie"> Об'єкт Movie </param>
    /// <param name="ct">CancellationToken</param>
    /// <returns> Об'єкт Movie </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieDetailDto>> Create([FromBody] CreateMovieRequest movie, CancellationToken ct)
    {
        // var validationResult = _createMovieValidator.Validate(movie);
        // if (!validationResult.IsValid)
        // {
        //     return ValidationProblem(
        //         title: "Помилки валідації",
        //         detail: "Один або декалька параметрів не валідні",
        //         statusCode: StatusCodes.Status400BadRequest    
        //     );
        // }


        var newMovie = await _movieService.CreateMovie(movie, ct);
        return CreatedAtAction(nameof(GetById), new { id = newMovie.Id }, newMovie);
    }

    /// <summary>
    /// Оновлює фільм
    /// </summary>
    /// <param name="id"> Ідентифікатор фільму</param>
    /// <param name="movie"> Об'єкт Movie</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns> 204 NoContent або помилка</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMovieRequest movie, CancellationToken ct)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var result = await _movieService.UpdateMovie(id, movie, ct);

        if (!result)
        {
            return NotFound(new { error = "Movie not found" });
        }

        return NoContent();
    }



    /// <summary>
    /// Оновлює фільм частково
    /// </summary>
    /// <param name="id"> Ідентифікатор фільму</param>
    /// <param name="movie"> Об'єкт Movie</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns> 204 NoContent або помилка</returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PartialUpdate(int id, [FromBody] UpdateMovieRequest movie, CancellationToken ct)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var result = await _movieService.PartialUpdateMovie(id, movie, ct);

        if (!result)
        {
            return NotFound(new { error = "Movie not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Видаляє фільм
    /// </summary>
    /// <param name="id"> Ідентифікатор фільму</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns> 204 NoContent або помилка</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _movieService.DeleteMovie(id, ct);
        return NoContent();
    }
}



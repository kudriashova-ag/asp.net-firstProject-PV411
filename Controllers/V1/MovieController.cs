using System.Text;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.DTOs.Movie;
using MyApp.Models;
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
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;

    public MovieController(IMapper mapper, AppDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }


    /// <summary>
    /// Повертає всі фільми
    /// </summary>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Список фільмів</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public async Task<ActionResult<IEnumerable<MovieSummaryDto>>> Get(CancellationToken ct)
    {
        var movies = await _db.Movies.AsNoTracking().ToListAsync(ct);
        var movieSummary = _mapper.Map<IEnumerable<MovieSummaryDto>>(movies);

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

        var movie = await _db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);

        if (movie == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        var movieDetail = _mapper.Map<MovieDetailDto>(movie);

        return Ok(movieDetail);
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
        var newMovie = _mapper.Map<Movie>(movie);

        _db.Movies.Add(newMovie);
        await _db.SaveChangesAsync(ct);

        var movieDetail = _mapper.Map<MovieDetailDto>(newMovie);
        return CreatedAtAction(nameof(GetById), new { id = movieDetail.Id }, movieDetail);
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
        var movieToUpdate = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);

        if (movieToUpdate == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        _mapper.Map(movie, movieToUpdate);

        await _db.SaveChangesAsync(ct);

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
        var movieToUpdate = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);

        if (movieToUpdate == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        _mapper.Map(movie, movieToUpdate);

        await _db.SaveChangesAsync(ct);

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
        if (id <= 0)
        {
            return BadRequest(new { error = "Id must be greater than 0" });
        }

        var movie = await _db.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        _db.Movies.Remove(movie);

        await _db.SaveChangesAsync(ct);

        return NoContent();
    }
}



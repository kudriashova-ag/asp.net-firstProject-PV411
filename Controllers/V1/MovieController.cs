using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

    public MovieController(IMapper mapper)
    {
        _mapper = mapper;
    }

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

    public ActionResult<IEnumerable<MovieSummaryDto>> Get()
    {
        // var movieSummary = _movies.Select(m => new MovieSummaryDto
        // {
        //     Id = m.Id,
        //     Title = m.Title
        // });

        var movieSummary = _mapper.Map<IEnumerable<MovieSummaryDto>>(_movies);

        return Ok(movieSummary);
    }

    /// <summary>
    /// Повертає фільм за Id
    /// </summary>
    /// <param name="id">Ідентифікатор фільму</param>
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
    public ActionResult<MovieDetailDto> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "Id must be greater than 0" });
        }

        var movie = _movies.FirstOrDefault(m => m.Id == id);

        if (movie == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        // var movieDetail = new MovieDetailDto
        // {
        //     Id = movie.Id,
        //     Title = movie.Title,
        //     Year = movie.Year,
        //     Director = movie.Director,
        //     Genre = movie.Genre
        // };

        var movieDetail = _mapper.Map<MovieDetailDto>(movie);

        return Ok(movieDetail);
    }


    /// <summary>
    /// Повертає фільм за назвою
    /// </summary>
    /// <param name="title"> Назва фільму </param>
    /// <returns>Об'єкт Movie або помилка</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult<Movie> GetByTitle([FromQuery] string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return BadRequest(new { error = "Title is required" });
        }
        var movie = _movies.FirstOrDefault(m => m.Title == title);
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
    /// <returns> Об'єкт Movie </returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<MovieDetailDto> Create([FromBody] CreateMovieRequest movie)
    {
        if (_movies.Any(m => m.Title == movie.Title))
        {
            return Conflict(new { error = "Movie already exists" });
        }

        var newMovie = new Movie
        {
            Id = _movies.Max(m => m.Id) + 1,
            Title = movie.Title,
            Year = movie.Year,
            Director = movie.Director,
            Genre = movie.Genre
        };

        _movies.Add(newMovie);

        // var movieDetail = new MovieDetailDto
        // {
        //     Id = newMovie.Id,
        //     Title = newMovie.Title,
        //     Year = newMovie.Year,
        //     Director = newMovie.Director,
        //     Genre = newMovie.Genre
        // };

        var movieDetail = _mapper.Map<MovieDetailDto>(newMovie);


        return CreatedAtAction(nameof(GetById), new { id = movieDetail.Id }, movieDetail);
    }

    /// <summary>
    /// Оновлює фільм
    /// </summary>
    /// <param name="id"> Ідентифікатор фільму</param>
    /// <param name="movie"> Об'єкт Movie</param>
    /// <returns> 204 NoContent або помилка</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Update(int id, [FromBody] CreateMovieRequest movie)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var movieToUpdate = _movies.FirstOrDefault(m => m.Id == id);

        if (movieToUpdate == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        // movieToUpdate.Title = movie.Title;
        // movieToUpdate.Year = movie.Year;
        // movieToUpdate.Genre = movie.Genre;
        // movieToUpdate.Director = movie.Director;

        _mapper.Map(movie, movieToUpdate);

        return NoContent();
    }



    /// <summary>
    /// Оновлює фільм частково
    /// </summary>
    /// <param name="id"> Ідентифікатор фільму</param>
    /// <param name="movie"> Об'єкт Movie</param>
    /// <returns> 204 NoContent або помилка</returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult PartialUpdate(int id, [FromBody] UpdateMovieRequest movie)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var movieToUpdate = _movies.FirstOrDefault(m => m.Id == id);

        if (movieToUpdate == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        _mapper.Map(movie, movieToUpdate);

        return NoContent();
    }

    /// <summary>
    /// Видаляє фільм
    /// </summary>
    /// <param name="id"> Ідентифікатор фільму</param>
    /// <returns> 204 NoContent або помилка</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "Id must be greater than 0" });
        }

        var movie = _movies.FirstOrDefault(m => m.Id == id);

        if (movie == null)
        {
            return NotFound(new { error = "Movie not found" });
        }

        _movies.Remove(movie);

        return NoContent();
    }
}



using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;

namespace MyApp.Controllers;

[Route("api/[controller]")]
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK, "application/json")]

    public ActionResult<IEnumerable<Movie>> Get()
    {
        return Ok(_movies);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Movie> GetById(int id)
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

        return Ok(movie);
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Movie> Create([FromBody] Movie movie)
    {
        // не використовується в проєктах
        // if (movie.Genre.Length < 3)
        // {
        //     ModelState.AddModelError("Genre", "Genre must have min 3 characters");
        //     return BadRequest(ModelState);
        // }

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
        // return Created("/api/movie/" + newMovie.Id, newMovie);
        return CreatedAtAction(nameof(GetById), new { id = newMovie.Id }, newMovie);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Update(int id, [FromBody] Movie movie)
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

        movieToUpdate.Title = movie.Title;
        movieToUpdate.Year = movie.Year;
        movieToUpdate.Genre = movie.Genre;
        movieToUpdate.Director = movie.Director;

        return NoContent();
    }


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



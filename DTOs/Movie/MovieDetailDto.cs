using MyApp.DTOs.Actor;
using MyApp.DTOs.Director;

namespace MyApp.DTOs.Movie;


public class MovieDetailDto
{
    public MovieDetailDto() { } // ← має бути
  

    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public IEnumerable<ActorInMovieDto> Actors { get; set; }
    public DirectorDto? Director { get; set; }
}

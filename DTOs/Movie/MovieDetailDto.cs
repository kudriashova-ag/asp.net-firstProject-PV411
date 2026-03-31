using MyApp.DTOs.Actor;
using MyApp.DTOs.Director;

namespace MyApp.DTOs.Movie;


public record MovieDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Year { get; set; }
    public IEnumerable<ActorInMovieDto> Actors { get; set; } = null!;
    public DirectorDto? Director { get; set; }
}

using MyApp.DTOs.Actor;
using MyApp.DTOs.Director;

namespace MyApp.DTOs.Movie;

public record MovieDetailDto
(
    int Id,
    string Title,
    string Genre,
    int Year,
    IEnumerable<ActorInMovieDto> Actors,
    DirectorDto? Director = null
);


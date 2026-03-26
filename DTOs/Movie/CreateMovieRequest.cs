using System.ComponentModel.DataAnnotations;

namespace MyApp.DTOs.Movie;

public record CreateMovieRequest
(
    string Title,
    string Genre,
    int Year,
    int? DirectorId
);



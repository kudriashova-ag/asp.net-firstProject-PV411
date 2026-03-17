using System.ComponentModel.DataAnnotations;

namespace MyApp.DTOs.Movie;

public record CreateMovieRequest
(
    [Required(ErrorMessage = "Title is required!")]
    [MinLength(3)]
    [MaxLength(50)]
    string Title,

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Length of {0} must be {2} - {1}")]
    string Genre,

    [Required]
    [NotInFuture]
    int Year,

    int? DirectorId
);



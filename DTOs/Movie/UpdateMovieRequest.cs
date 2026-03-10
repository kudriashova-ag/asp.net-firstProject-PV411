using System.ComponentModel.DataAnnotations;

namespace MyApp.DTOs.Movie;

public class UpdateMovieRequest
{
    [MinLength(3)]
    [MaxLength(50)]
    public string? Title { get; set; } = null!;


    [StringLength(20, MinimumLength = 3, ErrorMessage = "Length of {0} must be {2} - {1}")]
    public string? Genre { get; set; } = null!;

    [NotInFuture]
    public int? Year { get; set; }

    public string? Director { get; set; } = null!;
}
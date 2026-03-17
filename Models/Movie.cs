using System.ComponentModel.DataAnnotations;

namespace MyApp.Models;

public class Movie
{
    public int Id { get; set; }

    /// <summary>
    /// The title of the movie
    /// </summary>
    [Required(ErrorMessage = "Title is required!")]
    [MinLength(3)]
    [MaxLength(50)]
    public string Title { get; set; } = null!;



    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Length of {0} must be {2} - {1}")]
    public string Genre { get; set; } = null!;


    [Required]
    [NotInFuture]
    public int Year { get; set; }

    public int? DirectorId { get; set; }
    public Director? Director { get; set; } // Navigation property

    public IEnumerable<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
}
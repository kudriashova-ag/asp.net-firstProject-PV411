namespace MyApp.DTOs.Movie;

public record UpdateMovieRequest
{
    public string? Title { get; set; }
    public string? Genre { get; set; }
    public int? Year { get; set; }
    public int? DirectorId { get; set; }
    public List<ActorMovieAssigment>? Actors { get; set; }
}
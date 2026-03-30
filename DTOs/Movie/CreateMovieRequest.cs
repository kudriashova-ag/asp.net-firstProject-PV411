namespace MyApp.DTOs.Movie;

public record CreateMovieRequest
{
    public required string Title { get; init; }
    public required string Genre { get; set; }
    public int Year { get; set; }
    public int? DirectorId { get; set; }
    public List<ActorMovieAssigment> Actors { get; set; } = new List<ActorMovieAssigment>();

}
public record ActorMovieAssigment
{
    public int ActorId { get; set; }
    public string? Role { get; set; }
}


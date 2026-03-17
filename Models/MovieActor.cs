using System.Text.Json.Serialization;

namespace MyApp.Models;

public class MovieActor
{
    public int MovieId { get; set; }
    public int ActorId { get; set; }

    public Movie Movie { get; set; } = null!;
    public Actor Actor { get; set; } = null!;

    public string? Role { get; set; }
}
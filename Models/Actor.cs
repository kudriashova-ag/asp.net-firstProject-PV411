using System.Text.Json.Serialization;

namespace MyApp.Models;

public class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();

    public IEnumerable<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
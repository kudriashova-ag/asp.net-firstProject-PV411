namespace MyApp.DTOs.Actor;

public class ActorInMovieDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Role { get; set; }
}
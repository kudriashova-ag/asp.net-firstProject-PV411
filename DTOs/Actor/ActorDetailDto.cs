namespace MyApp.DTOs.Actor;

public class ActorDetailDto
{
    public int Id { get; set; }
    public string? Role { get; set; } = null!;

    public ActorDetailDto(int id,  string? role) => (Id, Role) = (id, role);
}
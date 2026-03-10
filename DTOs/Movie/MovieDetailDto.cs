namespace MyApp.DTOs.Movie;


public class MovieDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int Year { get; set; }
    public string Director { get; set; } = null!;
}
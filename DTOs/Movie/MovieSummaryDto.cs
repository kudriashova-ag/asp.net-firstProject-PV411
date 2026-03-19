namespace MyApp.DTOs.Movie;

public record MovieSummaryDto
(
    int Id,
    string Title,
    string Genre,
    int Year
);
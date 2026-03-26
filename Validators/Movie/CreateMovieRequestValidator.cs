using FluentValidation;
using MyApp.DTOs.Movie;
using MyApp.Services;

namespace MyApp.Validators.Movie;

public class CreateMovieRequestValidator : AbstractValidator<CreateMovieRequest>
{

    private readonly IMovieService _movieService;

    public CreateMovieRequestValidator( IMovieService movieService )
    {
        _movieService = movieService;

        RuleFor(m => m.Title)
            .NotEmpty().WithMessage("Назва обов'язкова")
            .MaximumLength(100).WithMessage("Назва не може бути більше 100 символів");

        RuleFor(m => m.Genre)
            .NotEmpty().WithMessage("Жанр обов'язковий")
            .MaximumLength(50).WithMessage("Жанр не може бути більше 50 символів");

        RuleFor(m => m.Year)
            .InclusiveBetween(1888, DateTime.UtcNow.Year)
            .WithMessage("Рік не може бути менше 1888 та більше поточного");

        RuleFor(m => m.DirectorId)
            .GreaterThan(0)
            .WithMessage("Id повинно бути білше 0");
            // .MustAsync(async (directorId, ct) =>
            // {
            //     return await _movieService.DirectorExists(directorId, ct);
            // })
            // .WithMessage("Такого режисера не існує");
    }

}
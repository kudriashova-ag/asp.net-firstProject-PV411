using FluentValidation;
using MyApp.Features.Movies.Commands;
using MyApp.Repository;

namespace MyApp.Features.Movies.Shared;

public class MoviesRequestValidator : AbstractValidator<CreateMovieCommand>
{

    private readonly IMovieRepository _movieRepository;

    public MoviesRequestValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;

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
            .WithMessage("Id повинно бути білше 0")
            .MustAsync(async (directorId, ct) =>
            {
                return await _movieRepository.DirectorExists(directorId, ct);
            })
            .WithMessage("Такого режисера не існує")
            .When(m => m.DirectorId.HasValue);
    }

}
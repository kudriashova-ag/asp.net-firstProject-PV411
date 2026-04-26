using FluentValidation;
using Ganss.Xss;
using Microsoft.AspNetCore.Http.Features;
using MyApp.Filters;
using MyApp.Repository;
using MyApp.Services;
using MyApp.Validators.Movie;

namespace MyApp.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHtmlSanitizer>(_ =>
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            return sanitizer;
        });

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 5 * 1024 * 1024;
        });

        // services.Configure<PaginationSettings>(
        //     configuration.GetSection("PaginationSettings"));

        services.AddAutoMapper(cfg => { }, typeof(Program));

        services.AddValidatorsFromAssemblyContaining<CreateMovieRequestValidator>(includeInternalTypes: true);
        services.AddValidatorsFromAssemblyContaining<Program>();

        services.AddScoped<RoleService>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IMovieRepository, MovieRepository>();

        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });

        return services;
    }
}
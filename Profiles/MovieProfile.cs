using AutoMapper;
using MyApp.DTOs.Movie;
using MyApp.Models;

namespace MyApp.Profiles;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        //        Source -> Target (destination)
        CreateMap<Movie, MovieDetailDto>()
            .ForMember(dest => dest.Year, opts => opts.MapFrom(src => src.Year + 5))
            .AfterMap((src, dest) =>
            {
                if (src.Year < 1950)
                {
                    dest.Genre = "Classic " + dest.Genre;
                }
            });

        CreateMap<Movie, MovieSummaryDto>();

        CreateMap<CreateMovieRequest, Movie>();

        CreateMap<UpdateMovieRequest, Movie>()
             .ForMember(dest => dest.Year, opts => opts.Condition((src, dest, srcMember) => src.Year.HasValue))    // !!!!!
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


        // ForMember
        // .ForMember(dest => dest.Title, opts => opts.MapFrom(src => src.MovieName))
        // .ForMember(dest => dest.Id, opts => opts.Ignore())
        // .ForMember(dest => dest.Director, opts => opt.NullSubstitute("Unknown"))




        // Життєвий цикл



    }
}

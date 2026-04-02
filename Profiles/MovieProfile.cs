using AutoMapper;
using MyApp.DTOs.Actor;
using MyApp.DTOs.Director;
using MyApp.DTOs.Movie;
using MyApp.Models;

namespace MyApp.Profiles;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        CreateMap<Director, DirectorDto>();

        CreateMap<MovieActor, ActorInMovieDto>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.ActorId))
            .ForMember(dest => dest.FirstName, opts => opts.MapFrom(src => src.Actor.FirstName))
            .ForMember(dest => dest.LastName, opts => opts.MapFrom(src => src.Actor.LastName))
            .ForMember(dest => dest.Role, opts => opts.MapFrom(src => src.Role));

        //          src      dest  
        CreateMap<Movie, MovieDetailDto>()
            .ForMember(dest => dest.Director, opts => opts.MapFrom(src => src.Director))
            .ForMember(dest => dest.Actors, opts => opts.MapFrom(src =>
                    src.MovieActors.Select(ma => new ActorInMovieDto(
                        ma.ActorId,
                        ma.Actor.FirstName,
                        ma.Actor.LastName,
                        ma.Role
            ))));

        CreateMap<Movie, MovieSummaryDto>();

        CreateMap<CreateMovieRequest, Movie>()
            .ForMember(dest => dest.Actors, opts => opts.Ignore())
            .ForMember(dest => dest.MovieActors, opts => opts.Ignore());

        CreateMap<UpdateMovieRequest, Movie>()
            .ForMember(dest => dest.Actors, opts => opts.Ignore())
            .ForMember(dest => dest.MovieActors, opts => opts.Ignore())
            .ForMember(dest => dest.Year, opts => opts.PreCondition((src, dest, srcMember) => src.Year.HasValue)) 
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


    }
}

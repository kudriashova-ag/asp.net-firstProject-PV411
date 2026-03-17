using Microsoft.EntityFrameworkCore;
using MyApp.Models;

namespace MyApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<MovieActor> MovieActor { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Director)
            .WithMany(d => d.Movies)
            .HasForeignKey(m => m.DirectorId)
            .OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<MovieActor>()
           .HasKey(ma => new { ma.MovieId, ma.ActorId });

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        // seeding
        modelBuilder.Entity<Director>().HasData(
            new Director { Id = 1, FirstName = "Quentin", LastName = "Tarantino" },
            new Director { Id = 2, FirstName = "Steven", LastName = "Spielberg" }
        );

        modelBuilder.Entity<Movie>().HasData(
            new Movie { Id = 1, Title = "Pulp Fiction", Genre = "Crime", Year = 1994, DirectorId = 1 },
            new Movie { Id = 2, Title = "Schindler's List", Genre = "Drama", Year = 1993, DirectorId = 2 },
            new Movie { Id = 3, Title = "The Dark Knight", Genre = "Action", Year = 2008, DirectorId = 2 },
            new Movie { Id = 4, Title = "The Shawshank Redemption", Genre = "Drama", Year = 1994, DirectorId = 2 }
        );

        // === SEEDING (додаємо акторів + зв'язки) ===
        modelBuilder.Entity<Actor>().HasData(
            new Actor { Id = 1, FirstName = "Uma", LastName = "Thurman" },
            new Actor { Id = 2, FirstName = "Samuel", LastName = "L. Jackson" },
            new Actor { Id = 3, FirstName = "John", LastName = "Travolta" }
        );

        modelBuilder.Entity<MovieActor>().HasData(
            new MovieActor { MovieId = 1, ActorId = 1, Role = "Mia Wallace" },   // Pulp Fiction
            new MovieActor { MovieId = 1, ActorId = 2, Role = "Jules Winnfield" },
            new MovieActor { MovieId = 1, ActorId = 3, Role = "Vincent Vega" }
        );

    }

}

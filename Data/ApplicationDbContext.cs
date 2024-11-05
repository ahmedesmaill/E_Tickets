using System;
using E_Tickets.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E_Tickets.ViewModel;

namespace E_Tickets.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    
    public ApplicationDbContext()
    {
    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
    {
    }


    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<ActorMovie> ActorMovies { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true).Build();
        var connection = builder.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connection);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<ActorMovie>()
            .HasKey(e => new
            {
                e.ActorId,
                e.MovieId
            });

        modelBuilder.Entity<Movie>()
           .HasOne(am => am.Category)
           .WithMany(m => m.Movies)
           .HasForeignKey(am => am.CategoryId);

        modelBuilder.Entity<Movie>()
          .HasOne(am => am.Cinema)
          .WithMany(m => m.Movies)
          .HasForeignKey(am => am.CinemaId);




        modelBuilder.Entity<ActorMovie>()
            .HasOne(am => am.Movie)
            .WithMany(m => m.ActorMovies)
            .HasForeignKey(am => am.MovieId);

        modelBuilder.Entity<ActorMovie>()
           .HasOne(am => am.Actor)
           .WithMany(m => m.ActorMovies)
           .HasForeignKey(am => am.ActorId);




    }

public DbSet<E_Tickets.ViewModel.ProfileVM> ProfileVM { get; set; } = default!;

//public DbSet<E_Tickets.ViewModel.LoginVM> LoginVM { get; set; } = default!;

}

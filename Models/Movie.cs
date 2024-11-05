using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Tickets.Models;

public partial class Movie
{
    public int Id { get; set; }
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Name { get; set; }
    public string? Description { get; set; }
    [Range(0, 10000)]
    public double Price { get; set; }
    [ValidateNever]
    public string ImgUrl { get; set; }
    [ValidateNever]
    public string? TrailerUrl { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MovieStatus MovieStatus { get; set; }

    public int CinemaId { get; set; }
    public int CategoryId { get; set; }
    [ValidateNever]
    public Cinema Cinema { get; set; }
    [ValidateNever]
    public Category Category { get; set; }

    [ValidateNever]
    public ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
}
public enum MovieStatus
{
    Upcoming,
    Available,
    Expired,
}

using System;
using System.Collections.Generic;

namespace E_Tickets.Models;

public partial class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Bio { get; set; }
    public string ProfilePicture { get; set; }
    public string? News { get; set; }
    public  ICollection<ActorMovie> ActorMovies { get; set; } = new List<ActorMovie>();
}

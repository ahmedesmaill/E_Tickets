using System;
using System.Collections.Generic;

namespace E_Tickets.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public  ICollection<Movie> Movies { get; set; } = new List<Movie>();
}

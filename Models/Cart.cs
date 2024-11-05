using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Tickets.Models
{
    [PrimaryKey("MovieId", "ApplicationUserId")]
    public class Cart
    {
        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        [ValidateNever]
        public Movie Movie { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
    }
}

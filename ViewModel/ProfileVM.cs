using System.ComponentModel.DataAnnotations;

namespace E_Tickets.ViewModel
{
    public class ProfileVM
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

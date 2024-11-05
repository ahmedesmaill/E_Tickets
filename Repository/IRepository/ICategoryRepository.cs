using E_Tickets.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // any additional logic
    }
}

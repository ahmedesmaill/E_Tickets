using E_Tickets.Data;
using E_Tickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace E_Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(string? query = null, int pageNumber = 1)
        {
            const int pageSize = 8;

            IQueryable<Movie> moviesQuery = _dbContext.Movies.Include(e => e.Category).Include(e => e.Cinema);

            if (!string.IsNullOrEmpty(query))
            {
                moviesQuery = moviesQuery.Where(e => e.Name.Contains(query));
            }

            int totalMovies = moviesQuery.Count();
            int totalPages = (int)Math.Ceiling(totalMovies / (double)pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                return RedirectToAction("NotFound", "Home");
            }

            var movies = moviesQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            return View(movies);
        }

        public IActionResult Details(int id)
        {
            var movie = _dbContext.Movies
                .Include(e => e.Category)
                .Include(e => e.Cinema)
                .Include(e => e.ActorMovies).ThenInclude(e => e.Actor)
                .FirstOrDefault(w => w.Id == id);

            if (movie == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return View(movie);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

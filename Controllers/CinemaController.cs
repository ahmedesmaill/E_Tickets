using E_Tickets.Data;
using E_Tickets.Models;
using E_Tickets.Repository;
using E_Tickets.Repository.IRepository;
using E_Tickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole},{SD.companyRole}")]
    public class CinemaController : Controller
    {
        private readonly ICinemaRepository cinemaRepository;
        private readonly IMovieRepository movieRepository;

        public CinemaController(ICinemaRepository cinemaRepository,IMovieRepository movieRepository)
        {
            this.cinemaRepository = cinemaRepository;
            this.movieRepository = movieRepository;
        }

        public IActionResult Index()
        {
            var cinemas = cinemaRepository.Get([e=>e.Movies]).ToList();
            return View(cinemas);
        }
        public IActionResult Create()
        {
            Cinema cinema = new Cinema();
            return View(cinema);
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema)
        {
            if (ModelState.IsValid)
            {

                cinemaRepository.Create(cinema);
                cinemaRepository.Commit();
                TempData["success"] = "Cinema Added Successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(cinema);
        }

        public IActionResult Edit(int cinemaId)
        {
            var cinema = cinemaRepository.GetOne(expression: e => e.Id == cinemaId);
            return View(model: cinema);


        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema)
        {
            if (ModelState.IsValid)
            {
                //Category category = new() { Id = Id, Name = Name };
                cinemaRepository.Edit(cinema);
                cinemaRepository.Commit();
                return RedirectToAction(nameof(Index));
            }

            return View(cinema);
        }
        public IActionResult Delete(int cinemaId)
        {
            var cinema = cinemaRepository.GetOne(expression: e => e.Id == cinemaId);
            if (cinema == null)
                return RedirectToAction("NotFound", "Home");

            cinemaRepository.Delete(cinema);
            cinemaRepository.Commit();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult AllMovies(int id)
        {
            var movies = movieRepository.Get([e=>e.Category,e=>e.Cinema],expression:e=>e.CinemaId==id).ToList();
            return View(movies);
        }
    }

}

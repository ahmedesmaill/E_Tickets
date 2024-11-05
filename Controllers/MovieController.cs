using E_Tickets.Models;
using E_Tickets.Repository;
using E_Tickets.Repository.IRepository;
using E_Tickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole},{SD.companyRole}")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository movieRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ICinemaRepository cinemaRepository;

        public MovieController(IMovieRepository movieRepository,ICategoryRepository categoryRepository, ICinemaRepository cinemaRepository)
        {
            this.movieRepository = movieRepository;
            this.categoryRepository = categoryRepository;
            this.cinemaRepository = cinemaRepository;
        }

        public IActionResult Index(string? query = null, int pageNumber = 1)
        {
            const int pageSize = 8;

            IQueryable<Movie> moviesQuery = movieRepository.Get([e => e.Category, e => e.Cinema]);
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
         
        public IActionResult Create()
        {
            var categories = categoryRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            var cinemas = cinemaRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            ViewData["categories"] = categories;
            ViewData["cinemas"] = cinemas;
           

            return View(new Movie());
        }

        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile ImgUrl)
        {


            if (ModelState.IsValid)
            {
                if (ImgUrl.Length > 0) // 85896
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName); // "1.png"
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImgUrl.CopyTo(stream);
                    }

                    movie.ImgUrl = fileName;

                    //return PhotoUrl.FileName;
                }

                //if (fileName != null)
                //{
                //    product.PhotoUrl = fileName;
                //}
                //else
                //{
                //    product.PhotoUrl = " ";
                //}

                movieRepository.Create(movie);
                movieRepository.Commit();
                TempData["success"] = "Movie Added Successfully";
                return RedirectToAction(nameof(Index));
            }



            var categories = categoryRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            var cinemas = cinemaRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            ViewData["categories"] = categories;
            ViewData["cinemas"] = cinemas;


          
            return View(movie);
        }

        public IActionResult Edit(int movieId)
        {
            var categories = categoryRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            var cinemas = cinemaRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            ViewData["categories"] = categories;
            ViewData["cinemas"] = cinemas;
            var movie = movieRepository.GetOne(expression: e=>e.Id==movieId);

            
            

            if (movie != null)
                return View(model: movie);

            return RedirectToAction("NotFound", "Home");
        }

        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile ImgUrl)
        {
            var oldMovie = movieRepository.GetOne(expression:e => e.Id == movie.Id, tracked:false);
            ModelState.Remove("ImgUrl");
            //TryValidateModel(product);
            if (ModelState.IsValid)
            {
                if (ImgUrl != null && ImgUrl.Length > 0) // 85896
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName); // "0283dasda2032-321321983lkjwlkds.png"

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", movie.ImgUrl);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImgUrl.CopyTo(stream);
                    }

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    movie.ImgUrl = fileName;
                }
                else
                {
                    movie.ImgUrl = oldMovie.ImgUrl;
                }

               movieRepository.Edit(movie);
                movieRepository.Commit();
                return RedirectToAction(nameof(Index));
            }


            movie.ImgUrl = oldMovie.ImgUrl;
            var categories = categoryRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            var cinemas = cinemaRepository.Get().Select(e => new SelectListItem { Text = e.Name, Value = e.Id.ToString() });
            ViewData["categories"] = categories;
            ViewData["cinemas"] = cinemas;
            return View(movie);
        }

        public IActionResult Delete(int movieId)
        {
            var oldmovie = movieRepository.GetOne(expression:e=>e.Id==movieId);

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", oldmovie.ImgUrl);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            movieRepository.Delete(oldmovie);
            movieRepository.Commit();
            return RedirectToAction(nameof(Index));
        }


    }
}





//
//private string? UploadImg(IFormFile PhotoUrl)
//{
//    // save in wwwroot
//    if (PhotoUrl.Length > 0) // 85896
//    {
//        var fileName = PhotoUrl.FileName; // "1.png"
//        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

//        using (var stream = System.IO.File.Create(filePath))
//        {
//            PhotoUrl.CopyTo(stream);
//        }
//        return PhotoUrl.FileName;
//    }
//    return null;
//}
//    }

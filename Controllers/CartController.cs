using E_Tickets.Models;
using E_Tickets.Repository;
using E_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Tickets.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMovieRepository movieRepository;

        public CartController(ICartRepository cartRepository, UserManager<ApplicationUser> userManager, IMovieRepository movieRepository)
        {
            this.cartRepository = cartRepository;
            this.userManager = userManager;
            this.movieRepository = movieRepository;
        }

        
        public IActionResult Index()
        {
            var appUser = userManager.GetUserId(User);

            var shoppingCarts = cartRepository.Get([e => e.Movie, e => e.ApplicationUser], e => e.ApplicationUserId == appUser).ToList();

            ViewBag.TotalPrice = shoppingCarts.Sum(e => e.Movie.Price * e.Count);
            return View(shoppingCarts);
        }

        public IActionResult AddToCart(int movieId, int count)
        {
            // Get the current user ID
            var appUserId = userManager.GetUserId(User);
            if (appUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the item already exists in the user's cart
            var cartDb = cartRepository.GetOne(expression:e => e.MovieId == movieId && e.ApplicationUserId == appUserId, tracked: true);

            if (cartDb != null)
            {
                // Update the existing cart item count
                cartDb.Count += count;
                cartRepository.Edit(cartDb);
            }
            else
            {
                // Create a new cart item if it doesn't exist
                Cart cart = new()
                {
                    Count = count,
                    MovieId = movieId,
                    ApplicationUserId = appUserId
                };
                cartRepository.Create(cart);
            }

            // Save changes to the database
            cartRepository.Commit();

            TempData["success"] = "Added to Cart successfully";
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Increment(int movieId)
        {
            var appUser = userManager.GetUserId(User);
            var cartDB = cartRepository.GetOne(expression: e => e.MovieId == movieId && e.ApplicationUserId == appUser);

            if (cartDB != null)
            {
                cartDB.Count++;
            }
            cartRepository.Commit();

            return RedirectToAction("Index");
        }

        public IActionResult Decrement(int movieId)
        {
            var appUser = userManager.GetUserId(User);
            var cartDB = cartRepository.GetOne(expression: e => e.MovieId == movieId && e.ApplicationUserId == appUser);

            if (cartDB != null)
            {
                cartDB.Count--;
                if (cartDB.Count == 0)
                {
                    cartRepository.Delete(cartDB);

                }
            }
            cartRepository.Commit();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int movieId)
        {
            var appUser = userManager.GetUserId(User);
            var cartDB = cartRepository.GetOne(expression: e => e.MovieId == movieId && e.ApplicationUserId == appUser);

            if (cartDB != null)
            {
                cartRepository.Delete(cartDB);
            }
            cartRepository.Commit();

            return RedirectToAction("Index");
        }

        public IActionResult Pay()
        {
            var appUser = userManager.GetUserId(User);
            var cartDBs = cartRepository.Get(includeProps: [e => e.Movie], expression: e => e.ApplicationUserId == appUser).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Cart/CheckOutSuccess",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Cart/CancelCheckout",
            };

            foreach (var item in cartDBs)
            {
                var result = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                        },
                        UnitAmount = (long)item.Movie.Price * 100,
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(result);
            }

            var service = new SessionService();
            var session = service.Create(options);
            

            return Redirect(session.Url);
        }
        public IActionResult CheckOutSuccess()
        {
            var appUser = userManager.GetUserId(User);

            var shoppingCarts = cartRepository.Get([e => e.Movie, e => e.ApplicationUser], e => e.ApplicationUserId == appUser).ToList();

            ViewBag.TotalPrice = shoppingCarts.Sum(e => e.Movie.Price * e.Count);

            foreach (var item in shoppingCarts)
            {
                cartRepository.Delete(item);
            }
            cartRepository.Commit();
            return View(shoppingCarts);

        }
        public IActionResult CancelCheckout()
        {
            TempData["error"] = "Your payment was cancelled. Please try again if you'd like to complete your order.";
            return View();
        }

    }
}

using E_Tickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole},{SD.companyRole}")]
    public class ActorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

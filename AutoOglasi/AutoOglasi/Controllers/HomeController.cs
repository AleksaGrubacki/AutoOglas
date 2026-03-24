using Microsoft.AspNetCore.Mvc;

namespace AutoOglasi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Oglasi");
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace AutoOglasi.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Oglasi");

    public IActionResult Error() => View();
}

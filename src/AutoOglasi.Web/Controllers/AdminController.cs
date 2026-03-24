using AutoOglasi.BLL;
using AutoOglasi.Web.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace AutoOglasi.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IKorisnikService _korisnikService;

    public AdminController(IAdminService adminService, IKorisnikService korisnikService)
    {
        _adminService = adminService;
        _korisnikService = korisnikService;
    }

    private bool JeAdmin() => HttpContext.Session.GetString("KorisnikUloga") == "Admin";

    public async Task<IActionResult> Index()
    {
        if (!JeAdmin())
            return RedirectToAction("Index", "Oglasi");

        var statistika = await _adminService.GetStatistikaAsync();
        ViewBag.BrojOglasa = statistika.BrojOglasa;
        ViewBag.BrojKorisnika = statistika.BrojKorisnika;
        ViewBag.BrojAktivnih = statistika.BrojAktivnih;

        return View();
    }

    public async Task<IActionResult> Oglasi()
    {
        if (!JeAdmin())
            return RedirectToAction("Index", "Oglasi");

        var lista = await _adminService.GetSviOglasiAsync();
        return View(lista.Select(d => d.ToViewModel()));
    }

    public async Task<IActionResult> ObrisiOglas(int id)
    {
        if (!JeAdmin())
            return RedirectToAction("Index", "Oglasi");

        await _adminService.ObrisiOglasAsync(id);
        return RedirectToAction("Oglasi");
    }

    public async Task<IActionResult> ToggleOglas(int id)
    {
        if (!JeAdmin())
            return RedirectToAction("Index", "Oglasi");

        await _adminService.ToggleOglasAsync(id);
        return RedirectToAction("Oglasi");
    }

    public async Task<IActionResult> Korisnici()
    {
        if (!JeAdmin())
            return RedirectToAction("Index", "Oglasi");

        var lista = await _korisnikService.GetSveKorisnikeZaAdminAsync();
        return View(lista.Select(d => d.ToViewModel()));
    }

    public async Task<IActionResult> PrommeniUlogu(int id)
    {
        if (!JeAdmin())
            return RedirectToAction("Index", "Oglasi");

        await _korisnikService.PromeniUloguAsync(id);
        return RedirectToAction("Korisnici");
    }

    public async Task<IActionResult> ObrisiKorisnika(int id)
    {
        if (!JeAdmin())
            return RedirectToAction("Index", "Oglasi");

        var mojeId = HttpContext.Session.GetInt32("KorisnikId");
        await _korisnikService.ObrisiKorisnikaAsync(id, mojeId);
        return RedirectToAction("Korisnici");
    }
}

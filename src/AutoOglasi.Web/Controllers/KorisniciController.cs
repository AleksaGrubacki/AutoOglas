using AutoOglasi.BLL;
using AutoOglasi.Web.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace AutoOglasi.Web.Controllers;

public class KorisniciController : Controller
{
    private readonly IKorisnikService _korisnikService;

    public KorisniciController(IKorisnikService korisnikService)
    {
        _korisnikService = korisnikService;
    }

    public IActionResult Registracija()
    {
        if (HttpContext.Session.GetString("KorisnikEmail") != null)
            return RedirectToAction("Index", "Oglasi");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registracija(string ime, string prezime,
        string email, string lozinka, string lozinkaPotvrda)
    {
        var rezultat = await _korisnikService.RegistrujAsync(ime, prezime, email, lozinka, lozinkaPotvrda);
        if (!rezultat.Uspeh || rezultat.Korisnik == null)
        {
            ViewBag.Greska = rezultat.Greska;
            return View();
        }

        var k = rezultat.Korisnik;
        HttpContext.Session.SetString("KorisnikEmail", k.Email ?? "");
        HttpContext.Session.SetString("KorisnikIme", k.Ime ?? "");
        HttpContext.Session.SetString("KorisnikUloga", k.Uloga);
        HttpContext.Session.SetInt32("KorisnikId", k.Id);

        return RedirectToAction("Index", "Oglasi");
    }

    public IActionResult Prijava()
    {
        if (HttpContext.Session.GetString("KorisnikEmail") != null)
            return RedirectToAction("Index", "Oglasi");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Prijava(string email, string lozinka)
    {
        var rezultat = await _korisnikService.PrijaviAsync(email, lozinka);
        if (!rezultat.Uspeh || rezultat.Korisnik == null)
        {
            ViewBag.Greska = rezultat.Greska;
            return View();
        }

        var k = rezultat.Korisnik;
        HttpContext.Session.SetString("KorisnikEmail", k.Email ?? "");
        HttpContext.Session.SetString("KorisnikIme", k.Ime ?? "");
        HttpContext.Session.SetString("KorisnikUloga", k.Uloga);
        HttpContext.Session.SetInt32("KorisnikId", k.Id);

        return RedirectToAction("Index", "Oglasi");
    }

    public IActionResult Odjava()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Oglasi");
    }

    public async Task<IActionResult> Profil()
    {
        var id = HttpContext.Session.GetInt32("KorisnikId");
        if (id == null)
            return RedirectToAction("Prijava");

        var dto = await _korisnikService.GetProfilAsync(id.Value);
        if (dto == null)
            return RedirectToAction("Prijava");

        return View(dto.ToViewModel());
    }
}

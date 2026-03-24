using AutoOglasi.Data;
using AutoOglasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.Controllers
{
    public class KorisniciController : Controller
    {
        private readonly AutoOglasiContext _context;

        public KorisniciController(AutoOglasiContext context)
        {
            _context = context;
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
            if (lozinka != lozinkaPotvrda)
            {
                ViewBag.Greska = "Lozinke se ne poklapaju!";
                return View();
            }

            var postojeci = await _context.Korisnici
                .FirstOrDefaultAsync(k => k.Email == email);

            if (postojeci != null)
            {
                ViewBag.Greska = "Korisnik sa tim emailom već postoji!";
                return View();
            }

            var korisnik = new Korisnik
            {
                Ime = ime,
                Prezime = prezime,
                Email = email,
                LozinkaHash = BCrypt.Net.BCrypt.HashPassword(lozinka),
                Uloga = "User",
                DatumRegistracije = DateTime.Now
            };

            _context.Korisnici.Add(korisnik);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("KorisnikEmail", korisnik.Email ?? "");
            HttpContext.Session.SetString("KorisnikIme", korisnik.Ime ?? "");
            HttpContext.Session.SetString("KorisnikUloga", korisnik.Uloga);
            HttpContext.Session.SetInt32("KorisnikId", korisnik.Id);

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
            var korisnik = await _context.Korisnici
                .FirstOrDefaultAsync(k => k.Email == email);

            if (korisnik == null || !BCrypt.Net.BCrypt.Verify(lozinka, korisnik.LozinkaHash))
            {
                ViewBag.Greska = "Pogrešan email ili lozinka!";
                return View();
            }

            HttpContext.Session.SetString("KorisnikEmail", korisnik.Email ?? "");
            HttpContext.Session.SetString("KorisnikIme", korisnik.Ime ?? "");
            HttpContext.Session.SetString("KorisnikUloga", korisnik.Uloga);
            HttpContext.Session.SetInt32("KorisnikId", korisnik.Id);

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

            var korisnik = await _context.Korisnici
                .Include(k => k.Oglasi!)
                    .ThenInclude(o => o.Model!)
                    .ThenInclude(m => m.Marka)
                .Include(k => k.Oglasi!)
                    .ThenInclude(o => o.Slike)
                .FirstOrDefaultAsync(k => k.Id == id);

            return View(korisnik);
        }
    }
}
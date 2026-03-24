using AutoOglasi.Data;
using AutoOglasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.Controllers
{
    public class AdminController : Controller
    {
        private readonly AutoOglasiContext _context;

        public AdminController(AutoOglasiContext context)
        {
            _context = context;
        }

        private bool JeAdmin()
        {
            return HttpContext.Session.GetString("KorisnikUloga") == "Admin";
        }

        // Admin panel - pregled
        public async Task<IActionResult> Index()
        {
            if (!JeAdmin())
                return RedirectToAction("Index", "Oglasi");

            ViewBag.BrojOglasa = await _context.Oglasi.CountAsync();
            ViewBag.BrojKorisnika = await _context.Korisnici.CountAsync();
            ViewBag.BrojAktivnih = await _context.Oglasi.CountAsync(o => o.Aktivan);

            return View();
        }

        // Upravljanje oglasima
        public async Task<IActionResult> Oglasi()
        {
            if (!JeAdmin())
                return RedirectToAction("Index", "Oglasi");

            var oglasi = await _context.Oglasi
                .Include(o => o.Model!).ThenInclude(m => m.Marka)
                .Include(o => o.Korisnik)
                .OrderByDescending(o => o.DatumObjave)
                .ToListAsync();

            return View(oglasi);
        }

        // Obrisi oglas
        public async Task<IActionResult> ObrisiOglas(int id)
        {
            if (!JeAdmin())
                return RedirectToAction("Index", "Oglasi");

            var oglas = await _context.Oglasi.FindAsync(id);
            if (oglas != null)
            {
                _context.Oglasi.Remove(oglas);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Oglasi");
        }

        // Aktiviraj/Deaktiviraj oglas
        public async Task<IActionResult> ToggleOglas(int id)
        {
            if (!JeAdmin())
                return RedirectToAction("Index", "Oglasi");

            var oglas = await _context.Oglasi.FindAsync(id);
            if (oglas != null)
            {
                oglas.Aktivan = !oglas.Aktivan;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Oglasi");
        }

        // Upravljanje korisnicima
        public async Task<IActionResult> Korisnici()
        {
            if (!JeAdmin())
                return RedirectToAction("Index", "Oglasi");

            var korisnici = await _context.Korisnici
                .Include(k => k.Oglasi)
                .OrderBy(k => k.Ime)
                .ToListAsync();

            return View(korisnici);
        }

        // Promeni ulogu korisnika
        public async Task<IActionResult> PrommeniUlogu(int id)
        {
            if (!JeAdmin())
                return RedirectToAction("Index", "Oglasi");

            var korisnik = await _context.Korisnici.FindAsync(id);
            if (korisnik != null)
            {
                korisnik.Uloga = korisnik.Uloga == "Admin" ? "User" : "Admin";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Korisnici");
        }

        // Obrisi korisnika
        public async Task<IActionResult> ObrisiKorisnika(int id)
        {
            if (!JeAdmin())
                return RedirectToAction("Index", "Oglasi");

            var mojeId = HttpContext.Session.GetInt32("KorisnikId");
            if (id == mojeId)
                return RedirectToAction("Korisnici");

            var korisnik = await _context.Korisnici.FindAsync(id);
            if (korisnik != null)
            {
                _context.Korisnici.Remove(korisnik);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Korisnici");
        }
    }
}
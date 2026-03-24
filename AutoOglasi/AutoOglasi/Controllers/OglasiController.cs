using AutoOglasi.BLL;
using AutoOglasi.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoOglasi.Controllers
{
    public class OglasiController : Controller
    {
        private readonly IOglasService _oglasService;
        private readonly IWebHostEnvironment _env;

        public OglasiController(IOglasService oglasService, IWebHostEnvironment env)
        {
            _oglasService = oglasService;
            _env = env;
        }

        public async Task<IActionResult> Index(string? marka, string? gorivo,
            int? godisteOd, int? godisteDo, decimal? cenaOd, decimal? cenaDo)
        {
            var oglasi = await _oglasService.GetFilteredAsync(marka, gorivo, godisteOd, godisteDo, cenaOd, cenaDo);
            ViewBag.Marke = await _oglasService.GetMarkeSelectListAsync();
            ViewBag.Goriva = _oglasService.GetGoriva();
            ViewBag.Godine = _oglasService.GetGodine();

            return View(oglasi);
        }

        public async Task<IActionResult> Detalji(int id)
        {
            var oglas = await _oglasService.GetDetaljiAsync(id);

            if (oglas == null) return NotFound();

            ViewBag.MozeUrediti = HttpContext.Session.GetInt32("KorisnikId") == oglas.KorisnikId
                || HttpContext.Session.GetString("KorisnikUloga") == "Admin";

            return View(oglas);
        }

        public async Task<IActionResult> Novi()
        {
            if (HttpContext.Session.GetString("KorisnikEmail") == null)
                return RedirectToAction("Prijava", "Korisnici");

            await PopuniDropdowneAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novi(Oglas oglas, List<IFormFile>? slike)
        {
            if (HttpContext.Session.GetString("KorisnikEmail") == null)
                return RedirectToAction("Prijava", "Korisnici");

            if (ModelState.IsValid)
            {
                var korisnikId = HttpContext.Session.GetInt32("KorisnikId") ?? 1;
                var oglasId = await _oglasService.KreirajAsync(oglas, korisnikId);
                await _oglasService.DodajSlikeAsync(oglasId, slike, _env.WebRootPath);

                return RedirectToAction(nameof(Index));
            }
            await PopuniDropdowneAsync();
            return View(oglas);
        }

        public async Task<IActionResult> Uredi(int id)
        {
            if (HttpContext.Session.GetString("KorisnikEmail") == null)
                return RedirectToAction("Prijava", "Korisnici");

            var oglas = await _oglasService.GetByIdAsync(id);
            if (oglas == null)
                return NotFound();

            var mojId = HttpContext.Session.GetInt32("KorisnikId") ?? 0;
            var jeAdmin = HttpContext.Session.GetString("KorisnikUloga") == "Admin";
            if (oglas.KorisnikId != mojId && !jeAdmin)
                return RedirectToAction(nameof(Index));

            await PopuniDropdowneAsync();
            return View(oglas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Uredi(Oglas oglas, List<IFormFile>? slike)
        {
            if (HttpContext.Session.GetString("KorisnikEmail") == null)
                return RedirectToAction("Prijava", "Korisnici");

            var mojId = HttpContext.Session.GetInt32("KorisnikId") ?? 0;
            var jeAdmin = HttpContext.Session.GetString("KorisnikUloga") == "Admin";

            if (ModelState.IsValid)
            {
                var uspeh = await _oglasService.UrediAsync(oglas, mojId, jeAdmin);
                if (!uspeh)
                    return RedirectToAction(nameof(Index));

                await _oglasService.DodajSlikeAsync(oglas.Id, slike, _env.WebRootPath);
                return RedirectToAction(nameof(Detalji), new { id = oglas.Id });
            }

            await PopuniDropdowneAsync();
            return View(oglas);
        }

        public async Task<IActionResult> Obrisi(int id)
        {
            var obrisan = await _oglasService.ObrisiAsync(
                id,
                HttpContext.Session.GetInt32("KorisnikId"),
                HttpContext.Session.GetString("KorisnikUloga"));
            if (!obrisan) return NotFound();
            return RedirectToAction("Index");
        }

        private async Task PopuniDropdowneAsync()
        {
            ViewBag.Marke = await _oglasService.GetMarkeAsync();
            ViewBag.Modeli = await _oglasService.GetModeliAsync();
            ViewBag.Kategorije = await _oglasService.GetKategorijeAsync();
            ViewBag.Goriva = _oglasService.GetGoriva();
            ViewBag.Menjaci = _oglasService.GetMenjaci();
            ViewBag.Godine = _oglasService.GetGodine();
        }
    }
}
using AutoOglasi.Data;
using AutoOglasi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.Controllers
{
    public class OglasiController : Controller
    {
        private readonly AutoOglasiContext _context;
        private readonly IWebHostEnvironment _env;

        public OglasiController(AutoOglasiContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(string? marka, string? gorivo,
            int? godisteOd, int? godisteDo, decimal? cenaOd, decimal? cenaDo)
        {
            var oglasi = _context.Oglasi
                .Include(o => o.Model!).ThenInclude(m => m.Marka)
                .Include(o => o.Kategorija)
                .Include(o => o.Slike)
                .Where(o => o.Aktivan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(marka))
                oglasi = oglasi.Where(o => o.Model!.Marka!.Naziv == marka);
            if (!string.IsNullOrEmpty(gorivo))
                oglasi = oglasi.Where(o => o.Gorivo == gorivo);
            if (godisteOd.HasValue)
                oglasi = oglasi.Where(o => o.Godiste >= godisteOd.Value);
            if (godisteDo.HasValue)
                oglasi = oglasi.Where(o => o.Godiste <= godisteDo.Value);
            if (cenaOd.HasValue)
                oglasi = oglasi.Where(o => o.Cena >= cenaOd.Value);
            if (cenaDo.HasValue)
                oglasi = oglasi.Where(o => o.Cena <= cenaDo.Value);

            ViewBag.Marke = new SelectList(_context.Marke.OrderBy(m => m.Naziv), "Naziv", "Naziv");
            ViewBag.Goriva = new List<string> { "Benzin", "Dizel", "Električni", "Hibridni pogon", "Gas (TNG)", "Metan (CNG)" };
            ViewBag.Godine = Enumerable.Range(1970, DateTime.Now.Year - 1970 + 1).Reverse();

            return View(await oglasi.OrderByDescending(o => o.DatumObjave).ToListAsync());
        }

        public async Task<IActionResult> Detalji(int id)
        {
            var oglas = await _context.Oglasi
                .Include(o => o.Model!).ThenInclude(m => m.Marka)
                .Include(o => o.Kategorija)
                .Include(o => o.Korisnik)
                .Include(o => o.Slike)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (oglas == null) return NotFound();

            ViewBag.MozeUrediti = HttpContext.Session.GetInt32("KorisnikId") == oglas.KorisnikId
                || HttpContext.Session.GetString("KorisnikUloga") == "Admin";

            return View(oglas);
        }

        public IActionResult Novi()
        {
            if (HttpContext.Session.GetString("KorisnikEmail") == null)
                return RedirectToAction("Prijava", "Korisnici");

            PopuniDropdowne();
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
                oglas.DatumObjave = DateTime.Now;
                oglas.Aktivan = true;
                oglas.KorisnikId = HttpContext.Session.GetInt32("KorisnikId") ?? 1;
                _context.Add(oglas);
                await _context.SaveChangesAsync();

                if (slike != null && slike.Count > 0)
                {
                    var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "oglasi");
                    Directory.CreateDirectory(uploadFolder);
                    bool prva = true;

                    foreach (var slika in slike)
                    {
                        if (slika.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(slika.FileName);
                            var filePath = Path.Combine(uploadFolder, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await slika.CopyToAsync(stream);
                            }
                            _context.Slike.Add(new Slika
                            {
                                OglasId = oglas.Id,
                                PutanjaFajla = "/uploads/oglasi/" + fileName,
                                JeNaslovna = prva
                            });
                            prva = false;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            PopuniDropdowne();
            return View(oglas);
        }

        public async Task<IActionResult> Obrisi(int id)
        {
            var uloga = HttpContext.Session.GetString("KorisnikUloga");
            var mojId = HttpContext.Session.GetInt32("KorisnikId");

            var oglas = await _context.Oglasi.FindAsync(id);
            if (oglas == null) return NotFound();

            if (oglas.KorisnikId != mojId && uloga != "Admin")
                return RedirectToAction("Index");

            _context.Oglasi.Remove(oglas);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private void PopuniDropdowne()
        {
            ViewBag.Marke = _context.Marke.OrderBy(m => m.Naziv).ToList();
            ViewBag.Modeli = _context.Modeli.Include(m => m.Marka).OrderBy(m => m.Naziv).ToList();
            ViewBag.Kategorije = _context.Kategorije.ToList();
            ViewBag.Goriva = new List<string> { "Benzin", "Dizel", "Električni", "Hibridni pogon", "Gas (TNG)", "Metan (CNG)" };
            ViewBag.Menjaci = new List<string> { "Manuelni", "Automatski", "Poluautomatski" };
            ViewBag.Godine = Enumerable.Range(1970, DateTime.Now.Year - 1970 + 1).Reverse();
        }
    }
}
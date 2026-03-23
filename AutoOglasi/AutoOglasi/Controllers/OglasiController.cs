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

        public OglasiController(AutoOglasiContext context)
        {
            _context = context;
        }

        // Prikaz svih oglasa sa pretragom
        public async Task<IActionResult> Index(string marka, string gorivo,
            int? godisteOd, int? godisteDo, decimal? cenaOd, decimal? cenaDo)
        {
            var oglasi = _context.Oglasi
                .Include(o => o.Model)
                    .ThenInclude(m => m.Marka)
                .Include(o => o.Kategorija)
                .Where(o => o.Aktivan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(marka))
                oglasi = oglasi.Where(o => o.Model.Marka.Naziv == marka);

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
            ViewBag.Goriva = new SelectList(new[] { "Benzin", "Dizel", "Električni", "Hibridni", "Gas (TNG)", "Metan (CNG)" });
            ViewBag.Godine = Enumerable.Range(1970, DateTime.Now.Year - 1970 + 1).Reverse();

            return View(await oglasi.OrderByDescending(o => o.DatumObjave).ToListAsync());
        }

        // Detalji jednog oglasa
        public async Task<IActionResult> Detalji(int id)
        {
            var oglas = await _context.Oglasi
                .Include(o => o.Model)
                    .ThenInclude(m => m.Marka)
                .Include(o => o.Kategorija)
                .Include(o => o.Korisnik)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (oglas == null)
                return NotFound();

            return View(oglas);
        }

        // Forma za novi oglas - GET
        public IActionResult Novi()
        {
            PopuniDropdowne();
            return View();
        }

        // Forma za novi oglas - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novi(Oglas oglas)
        {
            if (ModelState.IsValid)
            {
                oglas.DatumObjave = DateTime.Now;
                oglas.Aktivan = true;
                oglas.KorisnikId = 1; // hardkodovano za sada
                _context.Add(oglas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopuniDropdowne();
            return View(oglas);
        }

        private void PopuniDropdowne()
        {
            ViewBag.Marke = _context.Marke.OrderBy(m => m.Naziv).ToList();
            ViewBag.Modeli = _context.Modeli.Include(m => m.Marka).OrderBy(m => m.Naziv).ToList();
            ViewBag.Kategorije = _context.Kategorije.ToList();
            ViewBag.Goriva = new[] { "Benzin", "Dizel", "Električni", "Hibridni", "Gas (TNG)", "Metan (CNG)" };
            ViewBag.Menjaci = new[] { "Manuelni", "Automatski", "Poluautomatski" };
            ViewBag.Godine = Enumerable.Range(1970, DateTime.Now.Year - 1970 + 1).Reverse();
        }
    }
}
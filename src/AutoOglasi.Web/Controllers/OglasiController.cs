using AutoOglasi.BLL;
using AutoOglasi.BLL.Dto;
using AutoOglasi.Web.Mapping;
using AutoOglasi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoOglasi.Web.Controllers;

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
        var dto = await _oglasService.GetFilteredAsync(marka, gorivo, godisteOd, godisteDo, cenaOd, cenaDo);
        var markeOpcije = await _oglasService.GetMarkeSelectOptionsAsync();
        ViewBag.Marke = new SelectList(markeOpcije, nameof(SelectOptionDto.Value), nameof(SelectOptionDto.Text));
        ViewBag.Goriva = _oglasService.GetGoriva();
        ViewBag.Godine = _oglasService.GetGodine();

        return View(dto.Select(d => d.ToViewModel()));
    }

    public async Task<IActionResult> Detalji(int id)
    {
        var d = await _oglasService.GetDetaljiAsync(id);
        if (d == null) return NotFound();

        var vm = d.ToViewModel();
        ViewBag.MozeUrediti = HttpContext.Session.GetInt32("KorisnikId") == vm.KorisnikId
            || HttpContext.Session.GetString("KorisnikUloga") == "Admin";

        return View(vm);
    }

    public async Task<IActionResult> Novi()
    {
        if (HttpContext.Session.GetString("KorisnikEmail") == null)
            return RedirectToAction("Prijava", "Korisnici");

        await PopuniDropdowneAsync();
        return View(new OglasFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Novi(OglasFormViewModel model, List<IFormFile>? slike)
    {
        if (HttpContext.Session.GetString("KorisnikEmail") == null)
            return RedirectToAction("Prijava", "Korisnici");

        ValidirajFormularOglasa(model);
        if (ModelState.IsValid)
        {
            var korisnikId = HttpContext.Session.GetInt32("KorisnikId") ?? 1;
            var oglasId = await _oglasService.KreirajAsync(model.ToUpsertDto(), korisnikId);
            await _oglasService.DodajSlikeAsync(oglasId, slike, _env.WebRootPath);

            return RedirectToAction(nameof(Index));
        }

        await PopuniDropdowneAsync();
        return View(model);
    }

    public async Task<IActionResult> Uredi(int id)
    {
        if (HttpContext.Session.GetString("KorisnikEmail") == null)
            return RedirectToAction("Prijava", "Korisnici");

        var dto = await _oglasService.GetZaFormularAsync(id);
        if (dto == null)
            return NotFound();

        var mojId = HttpContext.Session.GetInt32("KorisnikId") ?? 0;
        var oglasDetalji = await _oglasService.GetDetaljiAsync(id);
        var jeAdmin = HttpContext.Session.GetString("KorisnikUloga") == "Admin";
        if (oglasDetalji == null || (oglasDetalji.KorisnikId != mojId && !jeAdmin))
            return RedirectToAction(nameof(Index));

        await PopuniDropdowneAsync();
        await LoadPostojeceSlikeAsync(id);
        return View(dto.ToFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Uredi(OglasFormViewModel model, List<IFormFile>? slike)
    {
        if (HttpContext.Session.GetString("KorisnikEmail") == null)
            return RedirectToAction("Prijava", "Korisnici");

        var mojId = HttpContext.Session.GetInt32("KorisnikId") ?? 0;
        var jeAdmin = HttpContext.Session.GetString("KorisnikUloga") == "Admin";

        ValidirajFormularOglasa(model);
        if (ModelState.IsValid)
        {
            var uspeh = await _oglasService.UrediAsync(model.ToUpsertDto(), mojId, jeAdmin);
            if (!uspeh)
                return RedirectToAction(nameof(Index));

            await _oglasService.DodajSlikeAsync(model.Id, slike, _env.WebRootPath);
            return RedirectToAction(nameof(Detalji), new { id = model.Id });
        }

        await PopuniDropdowneAsync();
        await LoadPostojeceSlikeAsync(model.Id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ObrisiSliku(int slikaId, int oglasId)
    {
        if (HttpContext.Session.GetString("KorisnikEmail") == null)
            return RedirectToAction("Prijava", "Korisnici");

        var mojId = HttpContext.Session.GetInt32("KorisnikId") ?? 0;
        var jeAdmin = HttpContext.Session.GetString("KorisnikUloga") == "Admin";

        await _oglasService.ObrisiSlikuAsync(slikaId, oglasId, mojId, jeAdmin, _env.WebRootPath);
        return RedirectToAction(nameof(Uredi), new { id = oglasId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostaviNaslovnu(int slikaId, int oglasId)
    {
        if (HttpContext.Session.GetString("KorisnikEmail") == null)
            return RedirectToAction("Prijava", "Korisnici");

        var mojId = HttpContext.Session.GetInt32("KorisnikId") ?? 0;
        var jeAdmin = HttpContext.Session.GetString("KorisnikUloga") == "Admin";

        await _oglasService.PostaviNaslovnuAsync(slikaId, oglasId, mojId, jeAdmin);
        return RedirectToAction(nameof(Uredi), new { id = oglasId });
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
        var marke = await _oglasService.GetMarkeAsync();
        var modeli = await _oglasService.GetModeliAsync();
        var kategorije = await _oglasService.GetKategorijeAsync();

        // Pogledi koriste isključivo ViewModele (ne BLL DTO tipove).
        ViewBag.Marke = marke.Select(m => m.ToViewModel()).ToList();
        ViewBag.Modeli = modeli.Select(m => m.ToViewModel()).ToList();
        ViewBag.Kategorije = kategorije.Select(k => k.ToViewModel()).ToList();
        ViewBag.Goriva = _oglasService.GetGoriva();
        ViewBag.Menjaci = _oglasService.GetMenjaci();
        ViewBag.Godine = _oglasService.GetGodine();
    }

    private void ValidirajFormularOglasa(OglasFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Naslov))
            ModelState.AddModelError(nameof(OglasFormViewModel.Naslov), "Naslov je obavezan.");
        if (model.ModelId <= 0)
            ModelState.AddModelError(nameof(OglasFormViewModel.ModelId), "Izaberi marku i model.");
        if (model.KategorijaId <= 0)
            ModelState.AddModelError(nameof(OglasFormViewModel.KategorijaId), "Izaberi karoseriju.");
        if (model.Godiste < 1950 || model.Godiste > DateTime.Now.Year + 1)
            ModelState.AddModelError(nameof(OglasFormViewModel.Godiste), "Unesi ispravno godište.");
        if (string.IsNullOrWhiteSpace(model.Gorivo))
            ModelState.AddModelError(nameof(OglasFormViewModel.Gorivo), "Izaberi gorivo.");
        if (string.IsNullOrWhiteSpace(model.Menjac))
            ModelState.AddModelError(nameof(OglasFormViewModel.Menjac), "Izaberi menjač.");
        if (!model.Cena.HasValue || model.Cena <= 0)
            ModelState.AddModelError(nameof(OglasFormViewModel.Cena), "Unesi cenu veću od 0.");
        if (!model.Kilometraza.HasValue || model.Kilometraza < 0)
            ModelState.AddModelError(nameof(OglasFormViewModel.Kilometraza), "Unesi kilometražu.");
    }

    private async Task LoadPostojeceSlikeAsync(int oglasId)
    {
        var detalji = await _oglasService.GetDetaljiAsync(oglasId);
        ViewBag.PostojeceSlike = detalji?.Slike.Select(s => s.ToViewModel()).ToList()
            ?? new List<SlikaViewModel>();
    }
}

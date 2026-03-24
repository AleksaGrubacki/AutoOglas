using AutoOglasi.BLL.Dto;
using AutoOglasi.DAL;
using AutoOglasi.DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.BLL;

public class OglasService : IOglasService
{
    private readonly IOglasRepository _oglasRepository;

    public OglasService(IOglasRepository oglasRepository)
    {
        _oglasRepository = oglasRepository;
    }

    public async Task<List<OglasListaDto>> GetFilteredAsync(string? marka, string? gorivo, int? godisteOd, int? godisteDo, decimal? cenaOd, decimal? cenaDo)
    {
        var oglasi = _oglasRepository.QueryAktivniOglasi();

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

        var list = await oglasi.OrderByDescending(o => o.DatumObjave).ToListAsync();
        return list.Select(EntityDtoMapper.ToListaDto).ToList();
    }

    public async Task<OglasDetaljiDto?> GetDetaljiAsync(int id)
    {
        var o = await _oglasRepository.GetDetaljiAsync(id);
        return o == null ? null : EntityDtoMapper.ToDetaljiDto(o);
    }

    public async Task<OglasUpsertDto?> GetZaFormularAsync(int id)
    {
        var o = await _oglasRepository.GetByIdAsync(id);
        if (o == null) return null;
        return ToUpsertDto(o);
    }

    private static OglasUpsertDto ToUpsertDto(Oglas o) => new()
    {
        Id = o.Id,
        Naslov = o.Naslov,
        Opis = o.Opis,
        Cena = o.Cena,
        Godiste = o.Godiste,
        Kilometraza = o.Kilometraza,
        Gorivo = o.Gorivo,
        Menjac = o.Menjac,
        ModelId = o.ModelId,
        KategorijaId = o.KategorijaId
    };

    public async Task<int> KreirajAsync(OglasUpsertDto podaci, int korisnikId)
    {
        var oglas = new Oglas
        {
            Naslov = podaci.Naslov,
            Opis = podaci.Opis,
            Cena = podaci.Cena,
            Godiste = podaci.Godiste,
            Kilometraza = podaci.Kilometraza,
            Gorivo = podaci.Gorivo,
            Menjac = podaci.Menjac,
            ModelId = podaci.ModelId,
            KategorijaId = podaci.KategorijaId,
            DatumObjave = DateTime.Now,
            Aktivan = true,
            KorisnikId = korisnikId
        };

        await _oglasRepository.AddAsync(oglas);
        await _oglasRepository.SaveChangesAsync();
        return oglas.Id;
    }

    public async Task<bool> UrediAsync(OglasUpsertDto noviPodaci, int korisnikId, bool jeAdmin)
    {
        var postojeci = await _oglasRepository.GetByIdAsync(noviPodaci.Id);
        if (postojeci == null)
            return false;

        if (postojeci.KorisnikId != korisnikId && !jeAdmin)
            return false;

        postojeci.Naslov = noviPodaci.Naslov;
        postojeci.Opis = noviPodaci.Opis;
        postojeci.Cena = noviPodaci.Cena;
        postojeci.Godiste = noviPodaci.Godiste;
        postojeci.Kilometraza = noviPodaci.Kilometraza;
        postojeci.Gorivo = noviPodaci.Gorivo;
        postojeci.Menjac = noviPodaci.Menjac;
        postojeci.ModelId = noviPodaci.ModelId;
        postojeci.KategorijaId = noviPodaci.KategorijaId;

        await _oglasRepository.UpdateAsync(postojeci);
        await _oglasRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ObrisiAsync(int id, int? korisnikId, string? uloga)
    {
        var oglas = await _oglasRepository.GetByIdAsync(id);
        if (oglas == null)
            return false;

        if (oglas.KorisnikId != korisnikId && uloga != "Admin")
            return false;

        await _oglasRepository.DeleteAsync(oglas);
        await _oglasRepository.SaveChangesAsync();
        return true;
    }

    public async Task DodajSlikeAsync(int oglasId, List<IFormFile>? slike, string webRootPath)
    {
        if (slike == null || slike.Count == 0)
            return;

        var uploadFolder = Path.Combine(webRootPath, "uploads", "oglasi");
        Directory.CreateDirectory(uploadFolder);
        var entity = await _oglasRepository.GetByIdAsync(oglasId);
        bool prva = !(entity?.Slike?.Any() ?? false);

        foreach (var slika in slike)
        {
            if (slika.Length <= 0)
                continue;

            var fileName = Guid.NewGuid() + Path.GetExtension(slika.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await slika.CopyToAsync(stream);
            }

            await _oglasRepository.AddSlikaAsync(new Slika
            {
                OglasId = oglasId,
                PutanjaFajla = "/uploads/oglasi/" + fileName,
                JeNaslovna = prva
            });
            prva = false;
        }

        await _oglasRepository.SaveChangesAsync();
    }

    public async Task<bool> ObrisiSlikuAsync(int slikaId, int oglasId, int korisnikId, bool jeAdmin, string webRootPath)
    {
        var oglas = await _oglasRepository.GetByIdAsync(oglasId);
        if (oglas == null)
            return false;

        if (oglas.KorisnikId != korisnikId && !jeAdmin)
            return false;

        var slika = await _oglasRepository.GetSlikaByIdAsync(slikaId);
        if (slika == null || slika.OglasId != oglasId)
            return false;

        await _oglasRepository.DeleteSlikaAsync(slika);
        await _oglasRepository.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(slika.PutanjaFajla))
        {
            var relative = slika.PutanjaFajla.Replace("/uploads/", "", StringComparison.OrdinalIgnoreCase)
                .TrimStart('/', '\\')
                .Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRootPath, "uploads", relative);
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        var preostale = await _oglasRepository.GetSlikeByOglasIdAsync(oglasId);
        if (preostale.Count > 0 && !preostale.Any(s => s.JeNaslovna))
        {
            preostale[0].JeNaslovna = true;
            await _oglasRepository.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> PostaviNaslovnuAsync(int slikaId, int oglasId, int korisnikId, bool jeAdmin)
    {
        var oglas = await _oglasRepository.GetByIdAsync(oglasId);
        if (oglas == null)
            return false;

        if (oglas.KorisnikId != korisnikId && !jeAdmin)
            return false;

        var slike = await _oglasRepository.GetSlikeByOglasIdAsync(oglasId);
        if (slike.Count == 0)
            return false;

        if (slike.All(s => s.Id != slikaId))
            return false;

        foreach (var s in slike)
            s.JeNaslovna = s.Id == slikaId;

        await _oglasRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<MarkaDto>> GetMarkeAsync()
    {
        var marke = await _oglasRepository.GetMarkeAsync();
        return marke.Select(EntityDtoMapper.ToMarkaDto).ToList();
    }

    public async Task<List<ModelDto>> GetModeliAsync()
    {
        var modeli = await _oglasRepository.GetModeliAsync();
        return modeli.Select(EntityDtoMapper.ToModelDto).ToList();
    }

    public async Task<List<KategorijaDto>> GetKategorijeAsync()
    {
        var kategorije = await _oglasRepository.GetKategorijeAsync();
        return kategorije.Select(EntityDtoMapper.ToKategorijaDto).ToList();
    }

    public List<string> GetGoriva() => new() { "Benzin", "Dizel", "Električni", "Hibridni pogon", "Gas (TNG)", "Metan (CNG)" };

    public List<string> GetMenjaci() => new() { "Manuelni", "Automatski", "Poluautomatski" };

    public IEnumerable<int> GetGodine() => Enumerable.Range(1970, DateTime.Now.Year - 1970 + 1).Reverse();

    public async Task<List<SelectOptionDto>> GetMarkeSelectOptionsAsync()
    {
        var marke = await _oglasRepository.GetMarkeAsync();
        return marke
            .Select(m => new SelectOptionDto { Value = m.Naziv ?? "", Text = m.Naziv ?? "" })
            .ToList();
    }
}

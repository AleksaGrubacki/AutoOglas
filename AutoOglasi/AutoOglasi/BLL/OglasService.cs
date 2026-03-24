using AutoOglasi.DAL;
using AutoOglasi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.BLL
{
    public class OglasService : IOglasService
    {
        private readonly IOglasRepository _oglasRepository;

        public OglasService(IOglasRepository oglasRepository)
        {
            _oglasRepository = oglasRepository;
        }

        public async Task<List<Oglas>> GetFilteredAsync(string? marka, string? gorivo, int? godisteOd, int? godisteDo, decimal? cenaOd, decimal? cenaDo)
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

            return await oglasi.OrderByDescending(o => o.DatumObjave).ToListAsync();
        }

        public async Task<Oglas?> GetDetaljiAsync(int id) => await _oglasRepository.GetDetaljiAsync(id);

        public async Task<Oglas?> GetByIdAsync(int id) => await _oglasRepository.GetByIdAsync(id);

        public async Task<int> KreirajAsync(Oglas oglas, int korisnikId)
        {
            oglas.DatumObjave = DateTime.Now;
            oglas.Aktivan = true;
            oglas.KorisnikId = korisnikId;

            await _oglasRepository.AddAsync(oglas);
            await _oglasRepository.SaveChangesAsync();
            return oglas.Id;
        }

        public async Task<bool> UrediAsync(Oglas noviPodaci, int korisnikId, bool jeAdmin)
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
            bool prva = !((await _oglasRepository.GetByIdAsync(oglasId))?.Slike?.Any() ?? false);

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

        public async Task<List<Marka>> GetMarkeAsync() => await _oglasRepository.GetMarkeAsync();
        public async Task<List<Model>> GetModeliAsync() => await _oglasRepository.GetModeliAsync();
        public async Task<List<Kategorija>> GetKategorijeAsync() => await _oglasRepository.GetKategorijeAsync();

        public List<string> GetGoriva() => new() { "Benzin", "Dizel", "Električni", "Hibridni pogon", "Gas (TNG)", "Metan (CNG)" };
        public List<string> GetMenjaci() => new() { "Manuelni", "Automatski", "Poluautomatski" };
        public IEnumerable<int> GetGodine() => Enumerable.Range(1970, DateTime.Now.Year - 1970 + 1).Reverse();

        public async Task<SelectList> GetMarkeSelectListAsync()
        {
            var marke = await _oglasRepository.GetMarkeAsync();
            return new SelectList(marke, "Naziv", "Naziv");
        }
    }
}

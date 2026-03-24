using AutoOglasi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoOglasi.BLL
{
    public interface IOglasService
    {
        Task<List<Oglas>> GetFilteredAsync(string? marka, string? gorivo, int? godisteOd, int? godisteDo, decimal? cenaOd, decimal? cenaDo);
        Task<Oglas?> GetDetaljiAsync(int id);
        Task<Oglas?> GetByIdAsync(int id);
        Task<int> KreirajAsync(Oglas oglas, int korisnikId);
        Task<bool> UrediAsync(Oglas noviPodaci, int korisnikId, bool jeAdmin);
        Task<bool> ObrisiAsync(int id, int? korisnikId, string? uloga);
        Task DodajSlikeAsync(int oglasId, List<IFormFile>? slike, string webRootPath);
        Task<List<Marka>> GetMarkeAsync();
        Task<List<Model>> GetModeliAsync();
        Task<List<Kategorija>> GetKategorijeAsync();
        List<string> GetGoriva();
        List<string> GetMenjaci();
        IEnumerable<int> GetGodine();
        Task<SelectList> GetMarkeSelectListAsync();
    }
}

using AutoOglasi.BLL.Dto;
using Microsoft.AspNetCore.Http;

namespace AutoOglasi.BLL;

public interface IOglasService
{
    Task<List<OglasListaDto>> GetFilteredAsync(string? marka, string? gorivo, int? godisteOd, int? godisteDo, decimal? cenaOd, decimal? cenaDo);
    Task<OglasDetaljiDto?> GetDetaljiAsync(int id);
    Task<OglasUpsertDto?> GetZaFormularAsync(int id);
    Task<int> KreirajAsync(OglasUpsertDto podaci, int korisnikId);
    Task<bool> UrediAsync(OglasUpsertDto podaci, int korisnikId, bool jeAdmin);
    Task<bool> ObrisiAsync(int id, int? korisnikId, string? uloga);
    Task DodajSlikeAsync(int oglasId, List<IFormFile>? slike, string webRootPath);
    Task<bool> ObrisiSlikuAsync(int slikaId, int oglasId, int korisnikId, bool jeAdmin, string webRootPath);
    Task<bool> PostaviNaslovnuAsync(int slikaId, int oglasId, int korisnikId, bool jeAdmin);
    Task<List<MarkaDto>> GetMarkeAsync();
    Task<List<ModelDto>> GetModeliAsync();
    Task<List<KategorijaDto>> GetKategorijeAsync();
    List<string> GetGoriva();
    List<string> GetMenjaci();
    IEnumerable<int> GetGodine();
    Task<List<SelectOptionDto>> GetMarkeSelectOptionsAsync();
}

using AutoOglasi.BLL.Dto;
using AutoOglasi.DAL.Entities;

namespace AutoOglasi.BLL;

internal static class EntityDtoMapper
{
    public static SlikaDto ToDto(Slika s) => new()
    {
        Id = s.Id,
        OglasId = s.OglasId,
        PutanjaFajla = s.PutanjaFajla,
        JeNaslovna = s.JeNaslovna
    };

    public static MarkaDto? ToDto(Marka? m) =>
        m == null ? null : new MarkaDto { Id = m.Id, Naziv = m.Naziv };

    public static ModelDto? ToDto(Model? model) =>
        model == null
            ? null
            : new ModelDto
            {
                Id = model.Id,
                Naziv = model.Naziv,
                MarkaId = model.MarkaId,
                Marka = ToDto(model.Marka)
            };

    public static KategorijaDto? ToDto(Kategorija? k) =>
        k == null ? null : new KategorijaDto { Id = k.Id, Naziv = k.Naziv };

    public static KorisnikKratkoDto? ToKratkoDto(Korisnik? k) =>
        k == null
            ? null
            : new KorisnikKratkoDto
            {
                Id = k.Id,
                Ime = k.Ime,
                Prezime = k.Prezime,
                Email = k.Email
            };

    public static KorisnikDto ToKorisnikDto(Korisnik k) => new()
    {
        Id = k.Id,
        Ime = k.Ime,
        Prezime = k.Prezime,
        Email = k.Email,
        Uloga = k.Uloga,
        DatumRegistracije = k.DatumRegistracije
    };

    public static OglasListaDto ToListaDto(Oglas o) => new()
    {
        Id = o.Id,
        Naslov = o.Naslov,
        Cena = o.Cena,
        Godiste = o.Godiste,
        Gorivo = o.Gorivo,
        Menjac = o.Menjac,
        Kilometraza = o.Kilometraza,
        DatumObjave = o.DatumObjave,
        Model = ToDto(o.Model),
        Slike = o.Slike?.Select(ToDto).ToList() ?? new List<SlikaDto>()
    };

    public static OglasDetaljiDto ToDetaljiDto(Oglas o) => new()
    {
        Id = o.Id,
        Naslov = o.Naslov,
        Opis = o.Opis,
        Cena = o.Cena,
        Godiste = o.Godiste,
        Kilometraza = o.Kilometraza,
        Gorivo = o.Gorivo,
        Menjac = o.Menjac,
        KorisnikId = o.KorisnikId,
        DatumObjave = o.DatumObjave,
        Aktivan = o.Aktivan,
        Model = ToDto(o.Model),
        Kategorija = ToDto(o.Kategorija),
        Korisnik = ToKratkoDto(o.Korisnik),
        Slike = o.Slike?.Select(ToDto).ToList() ?? new List<SlikaDto>()
    };

    public static OglasAdminDto ToAdminDto(Oglas o) => new()
    {
        Id = o.Id,
        Naslov = o.Naslov,
        Cena = o.Cena,
        DatumObjave = o.DatumObjave,
        Aktivan = o.Aktivan,
        Model = ToDto(o.Model),
        Korisnik = ToKratkoDto(o.Korisnik)
    };

    public static OglasMiniDto ToMiniDto(Oglas o) => new()
    {
        Id = o.Id,
        Naslov = o.Naslov,
        Cena = o.Cena,
        Godiste = o.Godiste,
        Kilometraza = o.Kilometraza,
        Model = ToDto(o.Model),
        Slike = o.Slike?.Select(ToDto).ToList() ?? new List<SlikaDto>()
    };

    public static KorisnikProfilDto ToProfilDto(Korisnik k) => new()
    {
        Id = k.Id,
        Ime = k.Ime,
        Prezime = k.Prezime,
        Email = k.Email,
        DatumRegistracije = k.DatumRegistracije,
        Oglasi = k.Oglasi?.Select(ToMiniDto).ToList() ?? new List<OglasMiniDto>()
    };

    public static MarkaDto ToMarkaDto(Marka m) => new() { Id = m.Id, Naziv = m.Naziv };

    public static ModelDto ToModelDto(Model m) => new()
    {
        Id = m.Id,
        Naziv = m.Naziv,
        MarkaId = m.MarkaId,
        Marka = ToDto(m.Marka)
    };

    public static KategorijaDto ToKategorijaDto(Kategorija k) => new() { Id = k.Id, Naziv = k.Naziv };
}

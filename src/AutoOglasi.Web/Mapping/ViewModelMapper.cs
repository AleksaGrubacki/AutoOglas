using AutoOglasi.BLL.Dto;
using AutoOglasi.Web.ViewModels;

namespace AutoOglasi.Web.Mapping;

public static class ViewModelMapper
{
    public static SlikaViewModel ToViewModel(this SlikaDto d) => new()
    {
        Id = d.Id,
        OglasId = d.OglasId,
        PutanjaFajla = d.PutanjaFajla,
        JeNaslovna = d.JeNaslovna
    };

    public static MarkaViewModel ToViewModel(this MarkaDto d) => new()
    {
        Id = d.Id,
        Naziv = d.Naziv
    };

    public static ModelViewModel ToViewModel(this ModelDto d) => new()
    {
        Id = d.Id,
        Naziv = d.Naziv,
        MarkaId = d.MarkaId,
        Marka = d.Marka?.ToViewModel()
    };

    public static KategorijaViewModel ToViewModel(this KategorijaDto d) => new()
    {
        Id = d.Id,
        Naziv = d.Naziv
    };

    public static KorisnikKratkoViewModel ToViewModel(this KorisnikKratkoDto d) => new()
    {
        Id = d.Id,
        Ime = d.Ime,
        Prezime = d.Prezime,
        Email = d.Email
    };

    public static OglasViewModel ToViewModel(this OglasListaDto d) => new()
    {
        Id = d.Id,
        Naslov = d.Naslov,
        Cena = d.Cena,
        Godiste = d.Godiste,
        Gorivo = d.Gorivo,
        Menjac = d.Menjac,
        Kilometraza = d.Kilometraza,
        DatumObjave = d.DatumObjave,
        Model = d.Model?.ToViewModel(),
        Slike = d.Slike.Select(s => s.ToViewModel()).ToList()
    };

    public static OglasViewModel ToViewModel(this OglasDetaljiDto d) => new()
    {
        Id = d.Id,
        Naslov = d.Naslov,
        Opis = d.Opis,
        Cena = d.Cena,
        Godiste = d.Godiste,
        Kilometraza = d.Kilometraza,
        Gorivo = d.Gorivo,
        Menjac = d.Menjac,
        KorisnikId = d.KorisnikId,
        DatumObjave = d.DatumObjave,
        Aktivan = d.Aktivan,
        Model = d.Model?.ToViewModel(),
        Kategorija = d.Kategorija?.ToViewModel(),
        Korisnik = d.Korisnik?.ToViewModel(),
        Slike = d.Slike.Select(s => s.ToViewModel()).ToList()
    };

    public static OglasViewModel ToViewModel(this OglasAdminDto d) => new()
    {
        Id = d.Id,
        Naslov = d.Naslov,
        Cena = d.Cena,
        DatumObjave = d.DatumObjave,
        Aktivan = d.Aktivan,
        Model = d.Model?.ToViewModel(),
        Korisnik = d.Korisnik?.ToViewModel()
    };

    public static OglasMiniViewModel ToViewModel(this OglasMiniDto d) => new()
    {
        Id = d.Id,
        Naslov = d.Naslov,
        Cena = d.Cena,
        Godiste = d.Godiste,
        Kilometraza = d.Kilometraza,
        Model = d.Model?.ToViewModel(),
        Slike = d.Slike.Select(s => s.ToViewModel()).ToList()
    };

    public static KorisnikProfilViewModel ToViewModel(this KorisnikProfilDto d) => new()
    {
        Id = d.Id,
        Ime = d.Ime,
        Prezime = d.Prezime,
        Email = d.Email,
        DatumRegistracije = d.DatumRegistracije,
        Oglasi = d.Oglasi.Select(o => o.ToViewModel()).ToList()
    };

    public static KorisnikAdminRowViewModel ToViewModel(this KorisnikAdminDto d) => new()
    {
        Id = d.Id,
        Ime = d.Ime,
        Prezime = d.Prezime,
        Email = d.Email,
        Uloga = d.Uloga,
        DatumRegistracije = d.DatumRegistracije,
        BrojOglasa = d.BrojOglasa
    };

    public static OglasUpsertDto ToUpsertDto(this OglasFormViewModel vm) => new()
    {
        Id = vm.Id,
        Naslov = vm.Naslov,
        Opis = vm.Opis,
        Cena = vm.Cena ?? 0,
        Godiste = vm.Godiste,
        Kilometraza = vm.Kilometraza ?? 0,
        Gorivo = vm.Gorivo,
        Menjac = vm.Menjac,
        ModelId = vm.ModelId,
        KategorijaId = vm.KategorijaId
    };

    public static OglasFormViewModel ToFormViewModel(this OglasUpsertDto d) => new()
    {
        Id = d.Id,
        Naslov = d.Naslov,
        Opis = d.Opis,
        Cena = d.Cena,
        Godiste = d.Godiste,
        Kilometraza = d.Kilometraza,
        Gorivo = d.Gorivo,
        Menjac = d.Menjac,
        ModelId = d.ModelId,
        KategorijaId = d.KategorijaId
    };
}

using AutoOglasi.BLL.Dto;
using AutoOglasi.DAL;
using AutoOglasi.DAL.Entities;

namespace AutoOglasi.BLL;

public class KorisnikService : IKorisnikService
{
    private readonly IKorisnikRepository _korisnikRepository;

    public KorisnikService(IKorisnikRepository korisnikRepository)
    {
        _korisnikRepository = korisnikRepository;
    }

    public async Task<(bool Uspeh, string? Greska, KorisnikDto? Korisnik)> RegistrujAsync(
        string ime, string prezime, string email, string lozinka, string lozinkaPotvrda)
    {
        if (lozinka != lozinkaPotvrda)
            return (false, "Lozinke se ne poklapaju!", null);

        var postojeci = await _korisnikRepository.GetByEmailAsync(email);
        if (postojeci != null)
            return (false, "Korisnik sa tim emailom već postoji!", null);

        var korisnik = new Korisnik
        {
            Ime = ime,
            Prezime = prezime,
            Email = email,
            LozinkaHash = BCrypt.Net.BCrypt.HashPassword(lozinka),
            Uloga = "User",
            DatumRegistracije = DateTime.Now
        };

        await _korisnikRepository.AddAsync(korisnik);
        await _korisnikRepository.SaveChangesAsync();
        return (true, null, EntityDtoMapper.ToKorisnikDto(korisnik));
    }

    public async Task<(bool Uspeh, string? Greska, KorisnikDto? Korisnik)> PrijaviAsync(string email, string lozinka)
    {
        var korisnik = await _korisnikRepository.GetByEmailAsync(email);

        if (korisnik == null || !BCrypt.Net.BCrypt.Verify(lozinka, korisnik.LozinkaHash))
            return (false, "Pogrešan email ili lozinka!", null);

        return (true, null, EntityDtoMapper.ToKorisnikDto(korisnik));
    }

    public async Task<KorisnikProfilDto?> GetProfilAsync(int id)
    {
        var k = await _korisnikRepository.GetProfilAsync(id);
        return k == null ? null : EntityDtoMapper.ToProfilDto(k);
    }

    public async Task<List<KorisnikAdminDto>> GetSveKorisnikeZaAdminAsync()
    {
        var korisnici = await _korisnikRepository.GetAllWithOglasiAsync();
        return korisnici.Select(k => new KorisnikAdminDto
        {
            Id = k.Id,
            Ime = k.Ime,
            Prezime = k.Prezime,
            Email = k.Email,
            Uloga = k.Uloga,
            DatumRegistracije = k.DatumRegistracije,
            BrojOglasa = k.Oglasi?.Count ?? 0
        }).ToList();
    }

    public async Task<bool> PromeniUloguAsync(int id)
    {
        var korisnik = await _korisnikRepository.GetByIdAsync(id);
        if (korisnik == null)
            return false;

        korisnik.Uloga = korisnik.Uloga == "Admin" ? "User" : "Admin";
        await _korisnikRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ObrisiKorisnikaAsync(int id, int? mojeId)
    {
        var korisnik = await _korisnikRepository.GetByIdWithOglasiAsync(id);
        if (korisnik == null)
            return false;

        var oglasi = korisnik.Oglasi?.ToList() ?? new List<Oglas>();
        var slike = oglasi.SelectMany(o => o.Slike ?? new List<Slika>()).ToList();

        if (slike.Count > 0)
            await _korisnikRepository.DeleteSlikeAsync(slike);
        if (oglasi.Count > 0)
            await _korisnikRepository.DeleteOglaseAsync(oglasi);

        await _korisnikRepository.DeleteAsync(korisnik);
        await _korisnikRepository.SaveChangesAsync();
        return true;
    }
}

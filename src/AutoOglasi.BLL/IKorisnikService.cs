using AutoOglasi.BLL.Dto;

namespace AutoOglasi.BLL;

public interface IKorisnikService
{
    Task<(bool Uspeh, string? Greska, KorisnikDto? Korisnik)> RegistrujAsync(string ime, string prezime, string email, string lozinka, string lozinkaPotvrda);
    Task<(bool Uspeh, string? Greska, KorisnikDto? Korisnik)> PrijaviAsync(string email, string lozinka);
    Task<KorisnikProfilDto?> GetProfilAsync(int id);
    Task<List<KorisnikAdminDto>> GetSveKorisnikeZaAdminAsync();
    Task<bool> PromeniUloguAsync(int id);
    Task<bool> ObrisiKorisnikaAsync(int id, int? mojeId);
}

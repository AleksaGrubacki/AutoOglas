using AutoOglasi.Models;

namespace AutoOglasi.BLL
{
    public interface IKorisnikService
    {
        Task<(bool Uspeh, string? Greska, Korisnik? Korisnik)> RegistrujAsync(string ime, string prezime, string email, string lozinka, string lozinkaPotvrda);
        Task<(bool Uspeh, string? Greska, Korisnik? Korisnik)> PrijaviAsync(string email, string lozinka);
        Task<Korisnik?> GetProfilAsync(int id);
        Task<List<Korisnik>> GetSveKorisnikeSaOglasimaAsync();
        Task<bool> PromeniUloguAsync(int id);
        Task<bool> ObrisiKorisnikaAsync(int id, int? mojeId);
    }
}

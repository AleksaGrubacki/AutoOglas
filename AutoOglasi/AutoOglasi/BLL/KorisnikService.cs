using AutoOglasi.DAL;
using AutoOglasi.Models;

namespace AutoOglasi.BLL
{
    public class KorisnikService : IKorisnikService
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikService(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }

        public async Task<(bool Uspeh, string? Greska, Korisnik? Korisnik)> RegistrujAsync(
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
            return (true, null, korisnik);
        }

        public async Task<(bool Uspeh, string? Greska, Korisnik? Korisnik)> PrijaviAsync(string email, string lozinka)
        {
            var korisnik = await _korisnikRepository.GetByEmailAsync(email);

            if (korisnik == null || !BCrypt.Net.BCrypt.Verify(lozinka, korisnik.LozinkaHash))
                return (false, "Pogrešan email ili lozinka!", null);

            return (true, null, korisnik);
        }

        public async Task<Korisnik?> GetProfilAsync(int id)
        {
            return await _korisnikRepository.GetProfilAsync(id);
        }

        public async Task<List<Korisnik>> GetSveKorisnikeSaOglasimaAsync()
        {
            return await _korisnikRepository.GetAllWithOglasiAsync();
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
            if (id == mojeId)
                return false;

            var korisnik = await _korisnikRepository.GetByIdAsync(id);
            if (korisnik == null)
                return false;

            await _korisnikRepository.DeleteAsync(korisnik);
            await _korisnikRepository.SaveChangesAsync();
            return true;
        }
    }
}

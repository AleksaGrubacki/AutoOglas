using AutoOglasi.DAL.Entities;

namespace AutoOglasi.DAL;

public interface IKorisnikRepository
{
    Task<Korisnik?> GetByEmailAsync(string email);
    Task<Korisnik?> GetByIdAsync(int id);
    Task<Korisnik?> GetByIdWithOglasiAsync(int id);
    Task<Korisnik?> GetProfilAsync(int id);
    Task<List<Korisnik>> GetAllWithOglasiAsync();
    Task AddAsync(Korisnik korisnik);
    Task DeleteOglaseAsync(IEnumerable<Oglas> oglasi);
    Task DeleteSlikeAsync(IEnumerable<Slika> slike);
    Task DeleteAsync(Korisnik korisnik);
    Task SaveChangesAsync();
}

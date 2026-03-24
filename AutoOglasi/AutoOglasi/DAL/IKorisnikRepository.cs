using AutoOglasi.Models;

namespace AutoOglasi.DAL
{
    public interface IKorisnikRepository
    {
        Task<Korisnik?> GetByEmailAsync(string email);
        Task<Korisnik?> GetByIdAsync(int id);
        Task<Korisnik?> GetProfilAsync(int id);
        Task<List<Korisnik>> GetAllWithOglasiAsync();
        Task AddAsync(Korisnik korisnik);
        Task DeleteAsync(Korisnik korisnik);
        Task SaveChangesAsync();
    }
}

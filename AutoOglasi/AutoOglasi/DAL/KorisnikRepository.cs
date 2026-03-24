using AutoOglasi.Data;
using AutoOglasi.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.DAL
{
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly AutoOglasiContext _context;

        public KorisnikRepository(AutoOglasiContext context)
        {
            _context = context;
        }

        public async Task<Korisnik?> GetByEmailAsync(string email)
        {
            return await _context.Korisnici.FirstOrDefaultAsync(k => k.Email == email);
        }

        public async Task<Korisnik?> GetByIdAsync(int id)
        {
            return await _context.Korisnici.FindAsync(id);
        }

        public async Task<Korisnik?> GetProfilAsync(int id)
        {
            return await _context.Korisnici
                .Include(k => k.Oglasi!)
                    .ThenInclude(o => o.Model!)
                    .ThenInclude(m => m.Marka)
                .Include(k => k.Oglasi!)
                    .ThenInclude(o => o.Slike)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<List<Korisnik>> GetAllWithOglasiAsync()
        {
            return await _context.Korisnici
                .Include(k => k.Oglasi)
                .OrderBy(k => k.Ime)
                .ToListAsync();
        }

        public async Task AddAsync(Korisnik korisnik)
        {
            await _context.Korisnici.AddAsync(korisnik);
        }

        public Task DeleteAsync(Korisnik korisnik)
        {
            _context.Korisnici.Remove(korisnik);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

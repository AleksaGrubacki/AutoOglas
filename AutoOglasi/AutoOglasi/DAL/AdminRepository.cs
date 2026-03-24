using AutoOglasi.Data;
using AutoOglasi.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.DAL
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AutoOglasiContext _context;

        public AdminRepository(AutoOglasiContext context)
        {
            _context = context;
        }

        public async Task<int> CountOglasiAsync() => await _context.Oglasi.CountAsync();
        public async Task<int> CountAktivniOglasiAsync() => await _context.Oglasi.CountAsync(o => o.Aktivan);
        public async Task<int> CountKorisniciAsync() => await _context.Korisnici.CountAsync();

        public async Task<List<Oglas>> GetAllOglasiAsync()
        {
            return await _context.Oglasi
                .Include(o => o.Model!).ThenInclude(m => m.Marka)
                .Include(o => o.Korisnik)
                .OrderByDescending(o => o.DatumObjave)
                .ToListAsync();
        }

        public async Task<Oglas?> GetOglasByIdAsync(int id) => await _context.Oglasi.FindAsync(id);

        public Task UpdateOglasAsync(Oglas oglas)
        {
            _context.Oglasi.Update(oglas);
            return Task.CompletedTask;
        }

        public Task DeleteOglasAsync(Oglas oglas)
        {
            _context.Oglasi.Remove(oglas);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}

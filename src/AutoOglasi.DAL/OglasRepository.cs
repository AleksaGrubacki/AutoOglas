using AutoOglasi.DAL.Data;
using AutoOglasi.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.DAL;

public class OglasRepository : IOglasRepository
{
    private readonly AutoOglasiContext _context;

    public OglasRepository(AutoOglasiContext context)
    {
        _context = context;
    }

    public IQueryable<Oglas> QueryAktivniOglasi()
    {
        return _context.Oglasi
            .Include(o => o.Model!).ThenInclude(m => m.Marka)
            .Include(o => o.Kategorija)
            .Include(o => o.Slike)
            .Where(o => o.Aktivan)
            .AsQueryable();
    }

    public async Task<Oglas?> GetDetaljiAsync(int id)
    {
        return await _context.Oglasi
            .Include(o => o.Model!).ThenInclude(m => m.Marka)
            .Include(o => o.Kategorija)
            .Include(o => o.Korisnik)
            .Include(o => o.Slike)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Oglas?> GetByIdAsync(int id)
    {
        return await _context.Oglasi
            .Include(o => o.Slike)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Marka>> GetMarkeAsync()
    {
        return await _context.Marke.OrderBy(m => m.Naziv).ToListAsync();
    }

    public async Task<List<Model>> GetModeliAsync()
    {
        return await _context.Modeli.Include(m => m.Marka).OrderBy(m => m.Naziv).ToListAsync();
    }

    public async Task<List<Kategorija>> GetKategorijeAsync()
    {
        return await _context.Kategorije.ToListAsync();
    }

    public async Task AddAsync(Oglas oglas)
    {
        await _context.Oglasi.AddAsync(oglas);
    }

    public Task UpdateAsync(Oglas oglas)
    {
        _context.Oglasi.Update(oglas);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Oglas oglas)
    {
        _context.Oglasi.Remove(oglas);
        return Task.CompletedTask;
    }

    public async Task AddSlikaAsync(Slika slika)
    {
        await _context.Slike.AddAsync(slika);
    }

    public async Task<Slika?> GetSlikaByIdAsync(int slikaId)
    {
        return await _context.Slike.FirstOrDefaultAsync(s => s.Id == slikaId);
    }

    public async Task<List<Slika>> GetSlikeByOglasIdAsync(int oglasId)
    {
        return await _context.Slike
            .Where(s => s.OglasId == oglasId)
            .OrderByDescending(s => s.JeNaslovna)
            .ThenBy(s => s.Id)
            .ToListAsync();
    }

    public Task DeleteSlikaAsync(Slika slika)
    {
        _context.Slike.Remove(slika);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

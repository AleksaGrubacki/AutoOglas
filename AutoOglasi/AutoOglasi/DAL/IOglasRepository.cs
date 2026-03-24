using AutoOglasi.Models;

namespace AutoOglasi.DAL
{
    public interface IOglasRepository
    {
        IQueryable<Oglas> QueryAktivniOglasi();
        Task<Oglas?> GetDetaljiAsync(int id);
        Task<Oglas?> GetByIdAsync(int id);
        Task<List<Marka>> GetMarkeAsync();
        Task<List<Model>> GetModeliAsync();
        Task<List<Kategorija>> GetKategorijeAsync();
        Task AddAsync(Oglas oglas);
        Task UpdateAsync(Oglas oglas);
        Task DeleteAsync(Oglas oglas);
        Task AddSlikaAsync(Slika slika);
        Task SaveChangesAsync();
    }
}

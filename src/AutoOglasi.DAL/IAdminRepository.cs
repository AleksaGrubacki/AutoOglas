using AutoOglasi.DAL.Entities;

namespace AutoOglasi.DAL;

public interface IAdminRepository
{
    Task<int> CountOglasiAsync();
    Task<int> CountAktivniOglasiAsync();
    Task<int> CountKorisniciAsync();
    Task<List<Oglas>> GetAllOglasiAsync();
    Task<Oglas?> GetOglasByIdAsync(int id);
    Task UpdateOglasAsync(Oglas oglas);
    Task DeleteOglasAsync(Oglas oglas);
    Task SaveChangesAsync();
}

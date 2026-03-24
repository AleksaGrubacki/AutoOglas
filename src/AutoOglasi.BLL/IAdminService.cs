using AutoOglasi.BLL.Dto;

namespace AutoOglasi.BLL;

public interface IAdminService
{
    Task<(int BrojOglasa, int BrojKorisnika, int BrojAktivnih)> GetStatistikaAsync();
    Task<List<OglasAdminDto>> GetSviOglasiAsync();
    Task<bool> ObrisiOglasAsync(int id);
    Task<bool> ToggleOglasAsync(int id);
}

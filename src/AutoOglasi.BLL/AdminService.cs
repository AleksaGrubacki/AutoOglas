using AutoOglasi.BLL.Dto;
using AutoOglasi.DAL;

namespace AutoOglasi.BLL;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<(int BrojOglasa, int BrojKorisnika, int BrojAktivnih)> GetStatistikaAsync()
    {
        var brojOglasa = await _adminRepository.CountOglasiAsync();
        var brojKorisnika = await _adminRepository.CountKorisniciAsync();
        var brojAktivnih = await _adminRepository.CountAktivniOglasiAsync();
        return (brojOglasa, brojKorisnika, brojAktivnih);
    }

    public async Task<List<OglasAdminDto>> GetSviOglasiAsync()
    {
        var oglasi = await _adminRepository.GetAllOglasiAsync();
        return oglasi.Select(EntityDtoMapper.ToAdminDto).ToList();
    }

    public async Task<bool> ObrisiOglasAsync(int id)
    {
        var oglas = await _adminRepository.GetOglasByIdAsync(id);
        if (oglas == null)
            return false;

        await _adminRepository.DeleteOglasAsync(oglas);
        await _adminRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleOglasAsync(int id)
    {
        var oglas = await _adminRepository.GetOglasByIdAsync(id);
        if (oglas == null)
            return false;

        oglas.Aktivan = !oglas.Aktivan;
        await _adminRepository.UpdateOglasAsync(oglas);
        await _adminRepository.SaveChangesAsync();
        return true;
    }
}

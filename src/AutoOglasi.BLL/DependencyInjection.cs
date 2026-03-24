using AutoOglasi.DAL;
using AutoOglasi.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoOglasi.BLL;

public static class DependencyInjection
{
    /// <summary>
    /// Registruje DbContext, repozitorijume i BLL servise (n-tier: poziva se iz Web composition root).
    /// </summary>
    public static IServiceCollection AddAutoOglasiBusinessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AutoOglasiContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("AutoOglasiConnection")));

        services.AddScoped<IOglasRepository, OglasRepository>();
        services.AddScoped<IKorisnikRepository, KorisnikRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();

        services.AddScoped<IOglasService, OglasService>();
        services.AddScoped<IKorisnikService, KorisnikService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}

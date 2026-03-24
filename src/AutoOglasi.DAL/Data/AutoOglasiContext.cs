using AutoOglasi.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoOglasi.DAL.Data;

public class AutoOglasiContext : DbContext
{
    public AutoOglasiContext(DbContextOptions<AutoOglasiContext> options)
        : base(options)
    {
    }

    public DbSet<Korisnik> Korisnici => Set<Korisnik>();
    public DbSet<Marka> Marke => Set<Marka>();
    public DbSet<Model> Modeli => Set<Model>();
    public DbSet<Kategorija> Kategorije => Set<Kategorija>();
    public DbSet<Oglas> Oglasi => Set<Oglas>();
    public DbSet<Slika> Slike => Set<Slika>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Korisnik>().ToTable("Korisnici");
        modelBuilder.Entity<Marka>().ToTable("Marke");
        modelBuilder.Entity<Model>().ToTable("Modeli");
        modelBuilder.Entity<Kategorija>().ToTable("Kategorije");
        modelBuilder.Entity<Oglas>(entity =>
        {
            entity.ToTable("Oglasi");
            entity.Property(e => e.Cena).HasPrecision(18, 2);
        });
        modelBuilder.Entity<Slika>().ToTable("Slike");
    }
}
